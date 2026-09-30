
using System.Text.Json;
using VirtualOperatorServer.CardMappings;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Configuration;
using VirtualOperatorServer.Coordinates;
using VirtualOperatorServer.Services;

namespace VirtualOperatorServer.Operation
{
    enum Action
    {
        Idle,
        Initializing,
        Swiping,
        Inserting,
        Extracting,
        Tapping,
        Barcoding,
        Touching
    }

    enum Position
    {
        Unknown,
        Idle,
        CardBay,
        Swipe,
        InsertStart,
        InsertEnd,
        Tap,
        BarCode,
        Touch
    }

    class Operations
    {
        private static readonly HashSet<byte> _stepperPowerIndices = [6, 7, 8, 9, 10, 11];
        private readonly object _dataLock = new();
        private readonly ILogger<Operations> _logger;
        private readonly BackSocket _backSocket;
        private readonly CoordinateStore _coordinateStore;
        private readonly CardMappingStore _cardMappingStore;
        private Action _action = Action.Idle;
        private Position _position = Position.Unknown;
        private readonly bool[,] _cardSlotsOccupied;

        public Operations(ILogger<Operations> logger, BackSocket backSocket, CoordinateStore coordinateStore,
            CardMappingStore cardMappingStore)
        {
            _logger = logger;
            _backSocket = backSocket;
            _coordinateStore = coordinateStore;
            _cardMappingStore = cardMappingStore;

            var coordinates = _coordinateStore.GetCoordinates();
            var cardSlotCount = coordinates.CardBays.Max(cardBay => cardBay.Cards.Count);
            _cardSlotsOccupied = new bool[coordinates.CardBays.Length, cardSlotCount];
            for (var bayIndex = 0; bayIndex < _cardSlotsOccupied.GetLength(0); bayIndex++)
            {
                for (var slotIndex = 0; slotIndex < _cardSlotsOccupied.GetLength(1); slotIndex++)
                    _cardSlotsOccupied[bayIndex, slotIndex] = true;
            }
        }

        public Position PositionState
        {
            get
            {
                lock (_dataLock)
                {
                    return _position;
                }
            }
        }
        
        public Action ActionState
        {
            get
            {
                lock (_dataLock)
                {
                    return _action;
                }
            }
        }

        public bool StartInitializingMachine()
        {
            Facade.StatusFacade.Facade? facade;
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position != Position.Unknown)
                    return false;

                facade = Facade.StatusFacade.GetFacade();
                if (facade == null)
                    return false;

                _action = Action.Initializing;
            }

            _ = Task.Run(() => initializeMachine(facade));
            return true;
        }

        private async Task<Facade.StatusFacade.Facade?> refreshFacade()
        {
            var command = new CmdGetStatus();
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            if (!command.ParseReply().result)
                return null;

            return Facade.StatusFacade.GetFacade();
        }

        private async Task<bool> powerOutput(byte index, bool enableOutput)
        {
            if (index >= Facade.StatusFacade.Facade.PowerOutputCount)
                return false;

            JsonElement payload = JsonSerializer.SerializeToElement(new
            {
                powerOutput = index,
                enable = enableOutput
            });
            var command = new CmdSetPowerOutput(payload);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task<bool> powerOutputBdc(bool enableOutput)
        {
            CmdSetGPIO.GPIO[] gpioArray =
            [
                new CmdSetGPIO.GPIO
                {
                    PortName = "PC",
                    BitIndex = 15,
                    Level = enableOutput ? (byte)1 : (byte)0
                }
            ];

            var command = new CmdSetGPIO(gpioArray);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        /// <summary>
        /// Sets the stepper's rampup, cruising, and rampdown pulse widths to the same value,
        /// with one accelerating step and one decelerating step. Returns false on any failure.
        /// </summary>
        private async Task<bool> stepperSetConstActivePeriod(byte index, ushort pulseWidth)
        {
            if (index >= Facade.StatusFacade.Facade.StepperCount)
                return false;

            try
            {
                async Task<bool> send(VirtualOperatorServer.CommandAndReply.CommandAndReply command)
                {
                    command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
                    return command.ParseReply().result;
                }

                ushort[] periods = [pulseWidth];
                if (!await send(new CmdSetStepperActiveRampupPulseWidth(index, 0, 1, periods)))
                    return false;

                if (!await send(new CmdSetStepperActiveCruisePulseWidth(index, pulseWidth)))
                    return false;

                return await send(new CmdSetStepperActiveRampdownPulseWidth(index, 0, 1, periods));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set constant active period for stepper {StepperIndex}.", index);
                return false;
            }
        }

        private async Task<bool> stepperSetDefaultActivePeriods(byte index)
        {
            const int maxPeriodsPerBatch = 100;

            var configs = StaticConfig.Instance.StepperConfigs;
            var status = CmdGetStatus.Status;
            if (index >= configs.Length || status == null)
                return false;

            var config = configs[index];
            var timer = config.timer;
            if (timer == Facade.StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
                return false;

            var activeConfig = config.activeModeConfig;
            if (activeConfig.acceleratingSteps < 1 || activeConfig.deacceleratingSteps < 1)
                return false;

            try
            {
                var timerClockPeriodNs = timer == Facade.StatusFacade.Facade.Stepper.Configuration.EnumTimer.FIX_TIMER
                    ? status.timersData[(int)timer].prescaler * 17u
                    : (uint)(status.timersData[(int)timer].prescaler * 4.16);

                var rampupPeriods = StepperPulsesFactory.Create(activeConfig.startingPulseWidth,
                                                                 activeConfig.acceleratingSteps,
                                                                 activeConfig.cruisingPulseWidth,
                                                                 timerClockPeriodNs).SCurvePulsesRampup;
                var rampdownPeriods = StepperPulsesFactory.Create(activeConfig.endingPulseWidth,
                                                                   activeConfig.deacceleratingSteps,
                                                                   activeConfig.cruisingPulseWidth,
                                                                   timerClockPeriodNs).SCurvePulsesRampdown;

                async Task<bool> send(VirtualOperatorServer.CommandAndReply.CommandAndReply command)
                {
                    command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
                    return command.ParseReply().result;
                }

                var totalBatches = (rampupPeriods.Count + maxPeriodsPerBatch - 1) / maxPeriodsPerBatch;
                for (var batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    var count = Math.Min(maxPeriodsPerBatch, rampupPeriods.Count - batchIndex * maxPeriodsPerBatch);
                    var batch = new ushort[count];
                    for (var i = 0; i < count; i++)
                        batch[i] = rampupPeriods[batchIndex * maxPeriodsPerBatch + i];

                    if (!await send(new CmdSetStepperActiveRampupPulseWidth(index, (byte)batchIndex, (byte)totalBatches, batch)))
                        return false;
                }

                if (!await send(new CmdSetStepperActiveCruisePulseWidth(index, activeConfig.cruisingPulseWidth)))
                    return false;

                totalBatches = (rampdownPeriods.Count + maxPeriodsPerBatch - 1) / maxPeriodsPerBatch;
                for (var batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    var count = Math.Min(maxPeriodsPerBatch, rampdownPeriods.Count - batchIndex * maxPeriodsPerBatch);
                    var batch = new ushort[count];
                    for (var i = 0; i < count; i++)
                        batch[i] = rampdownPeriods[batchIndex * maxPeriodsPerBatch + i];

                    if (!await send(new CmdSetStepperActiveRampdownPulseWidth(index, (byte)batchIndex, (byte)totalBatches, batch)))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set default active periods for stepper {StepperIndex}.", index);
                return false;
            }
        }

        private async Task<bool> stepperSetControls(byte index)
        {
            var configs = StaticConfig.Instance.StepperConfigs;
            if (index >= configs.Length)
                return false;

            var config = configs[index];
            var command = new CmdSetStepperControls(index,
                                                    config.isRisingEdgeDriven,
                                                    config.isForwardHigh,
                                                    config.isEnableHigh,
                                                    (byte)config.portHomeBoundary,
                                                    config.pinHomeBoundary,
                                                    (byte)config.portEndBoundary,
                                                    config.pinEndBoundary,
                                                    (byte)config.portEnable,
                                                    config.pinEnable,
                                                    (byte)config.portForward,
                                                    config.pinForward,
                                                    (byte)config.portClock,
                                                    config.pinClock,
                                                    config.homeBoundaryToReadySteps,
                                                    config.range,
                                                    config.stepsPerRotation,
                                                    (byte)config.encoder,
                                                    config.encoderCountsPerRotation,
                                                    config.encoderOffsetErrorThreshold);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task<bool> stepperEnable(byte index, bool isEnable)
        {
            if (index >= Facade.StatusFacade.Facade.StepperCount)
                return false;

            var command = new CmdSetStepperEnable(index, isEnable);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task<bool> stepperStartHomePositioning(byte index)
        {
            var configs = StaticConfig.Instance.StepperConfigs;
            if (index >= Facade.StatusFacade.Facade.StepperCount || index >= configs.Length)
                return false;

            var timer = configs[index].timer;
            if (timer == Facade.StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
                return false;

            var command = new CmdStartStepperHomePositioning(index, (byte)timer);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task<bool> stepperGoHome(byte index)
        {
            if (!await stepperSetDefaultActivePeriods(index))
            {
                _logger.LogError("Failed to set default active periods for stepper {StepperIndex}.", index);
                return false;
            }

            if (!await stepperSetControls(index))
            {
                _logger.LogError("Failed to set controls for stepper {StepperIndex}.", index);
                return false;
            }

            if (!await stepperEnable(index, true))
            {
                _logger.LogError("Failed to enable stepper {StepperIndex}.", index);
                return false;
            }

            if (!await stepperStartHomePositioning(index))
            {
                _logger.LogError("Failed to start home positioning for stepper {StepperIndex}.", index);
                return false;
            }

            return true;
        }

        private async Task<bool> stepperForward(byte index, bool isForward)
        {
            if (index >= Facade.StatusFacade.Facade.StepperCount)
                return false;

            var command = new CmdSetStepperForward(index, isForward);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task<bool> stepperCrossBoudnary(byte index)
        {
            var configs = StaticConfig.Instance.StepperConfigs;
            if (index >= Facade.StatusFacade.Facade.StepperCount || index >= configs.Length)
                return false;

            var config = configs[index].crossBoundary;
            var crossBoundary = new CmdSetStepperCrossBoundary.CrossBoudanry();
            if (crossBoundary.items.Length != config.boundaries.Length)
                return false;

            crossBoundary.enabled = config.enabled;
            crossBoundary.negativeRange = config.negativeRange;
            for (var i = 0; i < crossBoundary.items.Length; i++)
            {
                crossBoundary.items[i].enabled = config.boundaries[i].enabled;
                crossBoundary.items[i].offset = config.boundaries[i].value;
                crossBoundary.items[i].error = config.boundaries[i].error;
            }

            var command = new CmdSetStepperCrossBoundary(index, crossBoundary);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task<bool> stepperRunActive(byte index, uint steps)
        {
            var configs = StaticConfig.Instance.StepperConfigs;
            if (index >= Facade.StatusFacade.Facade.StepperCount || index >= configs.Length)
                return false;

            var config = configs[index];
            var timer = config.timer;
            if (timer == Facade.StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
                return false;

            if (!await stepperCrossBoudnary(index))
                return false;

            var setActiveCommand = new CmdSetStepperActive(index, steps);
            setActiveCommand.Reply = await _backSocket.SendAndReceiveAsync(setActiveCommand.Command);
            if (!setActiveCommand.ParseReply().result)
                return false;

            var runActiveCommand = new CmdRunStepperActive(index, (byte)timer);
            runActiveCommand.Reply = await _backSocket.SendAndReceiveAsync(runActiveCommand.Command);
            return runActiveCommand.ParseReply().result;
        }

        /// <summary>
        /// Sets the direction of the indexed stepper, runs the requested number of steps,
        /// and waits until it is ready, returning whether the movement succeeded.
        /// </summary>
        private async Task<bool> stepperMoveActive(byte index, bool isForward, uint steps)
        {
            if (!await stepperForward(index, isForward))
                return false;

            if (!await stepperRunActive(index, steps))
                return false;

            while (true)
            {
                await Task.Delay(200);

                var facade = await refreshFacade();
                if (facade == null)
                    return false;

                var state = facade.steppers[index].status.state;
                if (state == Facade.StatusFacade.Facade.Stepper.Status.State.READY)
                    return true;

                if (state != Facade.StatusFacade.Facade.Stepper.Status.State.RUNNING_ACTIVE)
                    return false;
            }
        }

        private async Task<(bool result, Facade.StatusFacade.Facade.Stepper.Status.State state)> stepperGetStatus(byte index)
        {
            if (index >= Facade.StatusFacade.Facade.StepperCount)
                return (false, default);

            var facade = await refreshFacade();
            if (facade == null)
                return (false, default);

            return (true, facade.steppers[index].status.state);
        }

        private async Task<bool> initializeStepper()
        {
            var idle = _coordinateStore.GetCoordinates().Idle;

            int[] idleOffsets = [idle.X, idle.Y, idle.Z, idle.U, idle.V];
            byte[] initializationOrder = [0, 2, 1, 3, 4];

            foreach (var index in initializationOrder)
            {
                if (!await stepperGoHome(index))
                {
                    _logger.LogError("Failed to home stepper {StepperIndex} during initialization.", index);
                    return false;
                }
                while (true)
                {
                    await Task.Delay(200);

                    var (result, state) = await stepperGetStatus(index);
                    if (!result)
                    {
                        _logger.LogError("Failed to retrieve the state of stepper {StepperIndex} during initialization.", index);
                        return false;
                    }

                    if (state == Facade.StatusFacade.Facade.Stepper.Status.State.READY)
                        break;

                    if (state != Facade.StatusFacade.Facade.Stepper.Status.State.RETURN_TO_HOME_BOUNDARY &&
                        state != Facade.StatusFacade.Facade.Stepper.Status.State.HOME_BOUNDARY_TO_READY)
                    {
                        _logger.LogError("Stepper {StepperIndex} entered unexpected state {StepperState} while homing.", index, state);
                        return false;
                    }
                }

                var offset = idleOffsets[index];
                if (!await stepperForward(index, offset >= 0))
                {
                    _logger.LogError("Failed to set direction for stepper {StepperIndex} during initialization.", index);
                    return false;
                }

                if (!await stepperRunActive(index, (uint)Math.Abs((long)offset)))
                {
                    _logger.LogError("Failed to move stepper {StepperIndex} to idle offset {IdleOffset} during initialization.", index, offset);
                    return false;
                }

                while (true)
                {
                    await Task.Delay(200);

                    var (result, state) = await stepperGetStatus(index);
                    if (!result)
                    {
                        _logger.LogError("Failed to retrieve the state of stepper {StepperIndex} while moving to its idle offset.", index);
                        return false;
                    }

                    if (state == Facade.StatusFacade.Facade.Stepper.Status.State.READY)
                        break;

                    if (state != Facade.StatusFacade.Facade.Stepper.Status.State.RUNNING_ACTIVE)
                    {
                        _logger.LogError("Stepper {StepperIndex} entered unexpected state {StepperState} while moving to its idle offset.", index, state);
                        return false;
                    }
                }
            }

            // clicker
            {
                const byte clickerIndex = 5;
                if (!await stepperGoHome(clickerIndex))
                {
                    _logger.LogError("Failed to home Clicker stepper during initialization.");
                    return false;
                }

                while (true)
                {
                    await Task.Delay(200);

                    var (result, state) = await stepperGetStatus(clickerIndex);
                    if (!result)
                    {
                        _logger.LogError("Failed to retrieve the state of the Clicker stepper during initialization.");
                        return false;
                    }

                    if (state == Facade.StatusFacade.Facade.Stepper.Status.State.READY)
                        break;

                    if (state != Facade.StatusFacade.Facade.Stepper.Status.State.RETURN_TO_HOME_BOUNDARY &&
                        state != Facade.StatusFacade.Facade.Stepper.Status.State.HOME_BOUNDARY_TO_READY)
                    {
                        _logger.LogError("Clicker stepper entered unexpected state {StepperState} while homing.", state);
                        return false;
                    }
                }

                var range = StaticConfig.Instance.StepperConfigs[clickerIndex].range - 1;
                if (!await stepperForward(clickerIndex, range >= 0))
                {
                    _logger.LogError("Failed to set direction for Clicker stepper during initialization.");
                    return false;
                }

                if (!await stepperRunActive(clickerIndex, range))
                {
                    _logger.LogError("Failed to move Clicker stepper to positive range {range} during initialization.", range);
                    return false;
                }

                while (true)
                {
                    await Task.Delay(200);

                    var (result, state) = await stepperGetStatus(clickerIndex);
                    if (!result)
                    {
                        _logger.LogError("Failed to retrieve the state of the Clicker stepper while moving to its positive range.");
                        return false;
                    }

                    if (state == Facade.StatusFacade.Facade.Stepper.Status.State.READY)
                        break;

                    if (state != Facade.StatusFacade.Facade.Stepper.Status.State.RUNNING_ACTIVE)
                    {
                        _logger.LogError("Clicker stepper entered unexpected state {StepperState} while moving to its positive range.", state);
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task initializeMachine(Facade.StatusFacade.Facade facade)
        {
            var succeeded = false;
            
            try
            {
                succeeded = true;
                foreach (var index in _stepperPowerIndices)
                {
                    if (!await powerOutput(index, true))
                    {
                        succeeded = false;
                        _logger.LogError("Failed to enable power output {StepperPowerIndex}.", index);
                    }
                }
                if (!await powerOutputBdc(true))
                {
                    succeeded = false;
                    _logger.LogError("Failed to enable BDC power output.");
                }

                await Task.Delay(500);

                // check power outputs.
                var refreshedFacade = await refreshFacade();
                if (refreshedFacade == null)
                {
                    succeeded = false;
                    _logger.LogError("Failed to refresh facade while validating power outputs.");
                }
                else
                {
                    foreach (var index in _stepperPowerIndices)
                    {
                        var output = refreshedFacade.powerOutputs[index];
                        if (!output.isEnabled || !output.isOutputDetected)
                        {
                            succeeded = false;
                            _logger.LogError("Power output {StepperPowerIndex} is not valid.", index);
                        }
                    }

                    if (!refreshedFacade.bdcPowerOutput.isEnabled || !refreshedFacade.bdcPowerOutput.isOutputDetected)
                    {
                        succeeded = false;
                        _logger.LogError("BDC power output is not valid.");
                    }
                }

                if (succeeded && !await initializeStepper())
                {
                    succeeded = false;
                    _logger.LogError("Failed to initialize steppers.");
                }

            }
            finally
            {
                lock (_dataLock)
                {
                    _action = Action.Idle;
                    if (succeeded)
                        _position = Position.Idle;
                }
            }
        }

        public bool StartSwipingCard(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Swiping;
            }

            _ = Task.Run(() => swipe(cardName, opt));
            return true;
        }

        private async Task<bool> pliersOpen()
        {
            return await pliersSetState(1, 0);
        }

        private async Task<bool> pliersClose()
        {
            return await pliersSetState(0, 1);
        }

        private async Task<bool> pliersDeactivate()
        {
            return await pliersSetState(0, 0);
        }

        private async Task<bool> pliersSetState(byte firstLevel, byte secondLevel)
        {
            CmdSetGPIO.GPIO[] gpioArray =
            [
                new CmdSetGPIO.GPIO { PortName = "PI", BitIndex = 9, Level = firstLevel },
                new CmdSetGPIO.GPIO { PortName = "PC", BitIndex = 13, Level = secondLevel }
            ];

            var command = new CmdSetGPIO(gpioArray);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task<int[]?> getCurrentStepperCoordinates()
        {
            var facade = await refreshFacade();
            if (facade == null)
            {
                _logger.LogError("Failed to retrieve current stepper positions.");
                return null;
            }

            return
            [
                facade.steppers[0].status.offset,
                facade.steppers[1].status.offset,
                facade.steppers[2].status.offset,
                facade.steppers[3].status.offset,
                facade.steppers[4].status.offset
            ];
        }

        private static int[] getPointCoordinates(Point point)
        {
            return [point.X, point.Y, point.Z, point.U, point.V];
        }

        /// <summary>
        /// Sets the direction of the indexed stepper from the sign of the requested distance,
        /// then starts it moving for the corresponding number of steps.
        /// </summary>
        /// <returns><see langword="true"/> when the move starts successfully; otherwise, <see langword="false"/>.</returns>
        private async Task<bool> stepperStartMove(byte index, long distance, string destination)
        {
            if (!await stepperForward(index, distance > 0))
            {
                _logger.LogError("Failed to set the direction of stepper {StepperIndex} while moving to {Destination}.", index, destination);
                return false;
            }

            if (!await stepperRunActive(index, (uint)Math.Abs(distance)))
            {
                _logger.LogError("Failed to start stepper {StepperIndex} while moving to {Destination}.", index, destination);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Waits for the indexed steppers to finish moving, failing if their states cannot be retrieved
        /// or if any stepper enters a state other than ready or actively running.
        /// </summary>
        /// <returns><see langword="true"/> when every indexed stepper is ready; otherwise, <see langword="false"/>.</returns>
        private async Task<bool> steppersWaitForMove(IReadOnlyList<byte> indices, string destination)
        {
            while (true)
            {
                await Task.Delay(200);

                var facade = await refreshFacade();
                if (facade == null)
                {
                    _logger.LogError("Failed to retrieve stepper states while moving to {Destination}.", destination);
                    return false;
                }

                var allReady = true;
                foreach (var index in indices)
                {
                    var state = facade.steppers[index].status.state;
                    if (state == Facade.StatusFacade.Facade.Stepper.Status.State.READY)
                        continue;

                    if (state != Facade.StatusFacade.Facade.Stepper.Status.State.RUNNING_ACTIVE)
                    {
                        _logger.LogError("Stepper {StepperIndex} entered unexpected state {StepperState} while moving to {Destination}.", index, state, destination);
                        return false;
                    }

                    allReady = false;
                }

                if (allReady)
                    return true;
            }
        }

        /// <summary>
        /// Starts each requested stepper moving from its current coordinate to its target in parallel,
        /// then waits for every moving stepper to become ready.
        /// </summary>
        /// <returns><see langword="true"/> when every move starts and completes successfully; otherwise, <see langword="false"/>.</returns>
        private async Task<bool> steppersMoveInParallel(
            IReadOnlyList<byte> indices,
            IReadOnlyList<int> currentCoordinates,
            IReadOnlyList<int> targetCoordinates,
            string destination)
        {
#if false
            var movingIndices = new List<byte>(indices.Count);
            var startTasks = new List<Task<bool>>(indices.Count);

            foreach (var index in indices)
            {
                var distance = (long)targetCoordinates[index] - currentCoordinates[index];
                if (distance == 0)
                    continue;

                movingIndices.Add(index);
                startTasks.Add(stepperStartMove(index, distance, destination));
            }

            if (startTasks.Count == 0)
                return true;

            var results = await Task.WhenAll(startTasks);
            foreach (var result in results)
            {
                if (!result)
                    return false;
            }

            return await steppersWaitForMove(movingIndices, destination);

#else
            var singleIndex = new List<byte>(1);
            singleIndex.Add(0);

            foreach (var index in indices)
            {
                var distance = (long)targetCoordinates[index] - currentCoordinates[index];
                if (distance == 0)
                    continue;

                var stepperStarted = await stepperStartMove(index, distance, destination);
                if (stepperStarted != true)
                {
                    _logger.LogError("Failed to start stepper {StepperIndex} to {Destination}", index, destination);
                    return false;
                }

                singleIndex[0] = index;

                var stepperArrived = await steppersWaitForMove(singleIndex, destination);
                if (stepperArrived != true)
                {
                    _logger.LogError("Failed to move stepper {StepperIndex} to {Destination}", index, destination);
                    return false;
                }
            }

            return true;
#endif
        }

        private async Task<bool> fromIdleToCardBay(byte bayIndex, byte cardIndex)
        {
            const string destination = "card bay";

            var currentCoordinates = await getCurrentStepperCoordinates();
            if (currentCoordinates == null)
            {
                _logger.LogError("Failed to retrieve current stepper coordinates while moving from idle to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            var coordinates = _coordinateStore.GetCoordinates();
            if (bayIndex >= coordinates.CardBays.Length)
            {
                _logger.LogError("Cannot move from idle to card bay {BayIndex}, slot {CardIndex}: the bay index is out of range.", bayIndex, cardIndex);
                return false;
            }

            if (!coordinates.CardBays[bayIndex].Cards.TryGetValue(cardIndex, out var target))
            {
                _logger.LogError("Cannot move from idle to card bay {BayIndex}, slot {CardIndex}: target coordinates are missing.", bayIndex, cardIndex);
                return false;
            }

            var targetCoordinates = getPointCoordinates(target);

            const byte zIndex = 2;
            if (!await steppersMoveInParallel([zIndex], currentCoordinates, targetCoordinates, destination))
            {
                _logger.LogError("Failed to move the Z stepper from idle to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            if (!await steppersMoveInParallel([0, 1, 3, 4], currentCoordinates, targetCoordinates, destination))
            {
                _logger.LogError("Failed to move the X, Y, U, V steppers from idle to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            return true;
        }

        private async Task<bool> fromSwipeToCardBay(byte bayIndex, byte cardIndex)
        {
            const string destination = "card bay";

            var currentCoordinates = await getCurrentStepperCoordinates();
            if (currentCoordinates == null)
            {
                _logger.LogError("Failed to retrieve current stepper coordinates while moving from swipe to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            var coordinates = _coordinateStore.GetCoordinates();
            if (bayIndex >= coordinates.CardBays.Length)
            {
                _logger.LogError("Cannot move from swipe to card bay {BayIndex}, slot {CardIndex}: the bay index is out of range.", bayIndex, cardIndex);
                return false;
            }

            if (!coordinates.CardBays[bayIndex].Cards.TryGetValue(cardIndex, out var target))
            {
                _logger.LogError("Cannot move from swipe to card bay {BayIndex}, slot {CardIndex}: target coordinates are missing.", bayIndex, cardIndex);
                return false;
            }

            var targetCoordinates = getPointCoordinates(target);

            // move Z
            if (!await steppersMoveInParallel([2], currentCoordinates, targetCoordinates, destination))
            {
                _logger.LogError("Failed to move the Z stepper from swipe to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            // move U, V
            if (!await steppersMoveInParallel([3, 4], currentCoordinates, targetCoordinates, destination))
            {
                _logger.LogError("Failed to move the U, V steppers from swipe to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            // move X, Y
            if (!await steppersMoveInParallel([0, 1], currentCoordinates, targetCoordinates, destination))
            {
                _logger.LogError("Failed to move the X, Y steppers from swipe to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            return true;
        }

        private async Task<bool> fromCardBayToCardBay(byte bayIndex, byte cardIndex)
        {
            const string destination = "card bay";

            var currentCoordinates = await getCurrentStepperCoordinates();
            if (currentCoordinates == null)
            {
                _logger.LogError("Failed to retrieve current stepper coordinates while moving from card bay to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            var coordinates = _coordinateStore.GetCoordinates();
            if (bayIndex >= coordinates.CardBays.Length)
            {
                _logger.LogError("Cannot move from card bay to card bay {BayIndex}, slot {CardIndex}: the bay index is out of range.", bayIndex, cardIndex);
                return false;
            }

            if (!coordinates.CardBays[bayIndex].Cards.TryGetValue(cardIndex, out var target))
            {
                _logger.LogError("Cannot move from card bay  to card bay {BayIndex}, slot {CardIndex}: target coordinates are missing.", bayIndex, cardIndex);
                return false;
            }

            var targetCoordinates = getPointCoordinates(target);

            if (!await steppersMoveInParallel([0, 1], currentCoordinates, targetCoordinates, destination))
            {
                _logger.LogError("Failed to move the X, Y steppers from card bay to card bay {BayIndex}, slot {CardIndex}.", bayIndex, cardIndex);
                return false;
            }

            return true;
        }

        // move to card bay 
        private async Task<bool> toCardBay(byte bayIndex, byte cardIndex)
        {
            switch (_position)
            {
                case Position.Idle:
                {
                    var succeeded = await fromIdleToCardBay(bayIndex, cardIndex);
                    lock (_dataLock)
                    {
                        _position = succeeded ? Position.CardBay : Position.Unknown;
                    }

                    return succeeded;
                }
                case Position.Swipe:
                {
                    var succeeded = await fromSwipeToCardBay(bayIndex, cardIndex);
                    lock (_dataLock)
                    {
                        _position = succeeded ? Position.CardBay : Position.Unknown;
                    }

                    return succeeded;
                }
                case Position.CardBay:
                {
                    var succeeded = await fromCardBayToCardBay(bayIndex, cardIndex);
                    lock (_dataLock)
                    {
                        _position = succeeded ? Position.CardBay : Position.Unknown;
                    }

                    return succeeded;
                }

                default:
                    return false;
            }
        }

        private async Task<bool> fetchCard()
        {
            await pliersOpen();
            await Task.Delay(200);

            var fetchDistance = _coordinateStore.GetCoordinates().FetchDistance;
            if (fetchDistance == 0)
            {
                _logger.LogError("Cannot fetch card: fetch distance is zero.");
                return false;
            }

            // going down to fetch the card
            const byte zIndex = 2;
            var steps = (uint)Math.Abs((long)fetchDistance);
            if (!await stepperMoveActive(zIndex, fetchDistance > 0, steps))
            {
                _logger.LogError("Failed to move stepper {StepperIndex} by fetch distance {FetchDistance} while fetching card.", zIndex, fetchDistance);
                return false;
            }

            await pliersClose();
            await Task.Delay(500);

            // going up to card position
            if (!await stepperMoveActive(zIndex, fetchDistance < 0, steps))
            {
                _logger.LogError("Failed to reverse stepper {StepperIndex} by fetch distance {FetchDistance} while fetching card.", zIndex, fetchDistance);
                return false;
            }            

            return true;
        }

        private async Task<bool> fromCardBayToMsrSwiping(uint opt)
        {
            try
            {
                var coordinates = _coordinateStore.GetCoordinates();
                if (opt >= (uint)coordinates.OPTs.Length)
                {
                    _logger.LogError("Cannot move to the MSR swiping position: option {Option} is invalid.", opt);
                    return false;
                }

                var target = coordinates.OPTs[(int)opt].MsrSwipingStartPoint;
                if (target == null)
                {
                    _logger.LogError("Cannot move to the MSR swiping position: option {Option} has no start point.", opt);
                    return false;
                }

                const string destination = "the MSR swiping position";
                var currentCoordinates = await getCurrentStepperCoordinates();
                if (currentCoordinates == null)
                    return false;
                var targetCoordinates = getPointCoordinates(target);

                // move X, Y
                if (!await steppersMoveInParallel([0, 1], currentCoordinates, targetCoordinates, destination))
                    return false;

                // move U, V
                if (!await steppersMoveInParallel([3, 4], currentCoordinates, targetCoordinates, destination))
                    return false;

                // move Z
                return await steppersMoveInParallel([2], currentCoordinates, targetCoordinates, destination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to move to the MSR swiping position using option {Option}.", opt);
                return false;
            }
        }

        private async Task<bool> toMsrSwiping(uint opt)
        {
            switch (_position)
            {
                case Position.CardBay:
                {
                    var succeeded = await fromCardBayToMsrSwiping(opt);
                    lock (_dataLock)
                    {
                        _position = succeeded ? Position.Swipe : Position.Unknown;
                    }

                    return succeeded;
                }

                default:
                    return false;
            }
        }

        private async Task<bool> swipeCard(uint opt)
        {
            if (_position != Position.Swipe)
            {
                _logger.LogError("Failure in swipeCard, not in Swipe position");
                return false;
            }

            var coordinates = _coordinateStore.GetCoordinates();
            if (opt >= (uint)coordinates.OPTs.Length)
            {
                _logger.LogError("Cannot swipe card: option {Option} is invalid.", opt);
                return false;
            }

            const byte xIndex = 0;
            const byte yIndex = 1;

            var settings = coordinates.OPTs[(int)opt];
            var swipeDeltaX = (uint)Math.Abs((long)settings.SwipeDeltaX);
            var swipeDeltaY = (uint)Math.Abs((long)settings.SwipeDeltaY);

            if (!await stepperMoveActive(yIndex, true, swipeDeltaY))
                return false;

            if (!await stepperMoveActive(xIndex, false, swipeDeltaX))
                return false;

            return await stepperMoveActive(yIndex, false, swipeDeltaY);
        }

        private async Task<bool> replaceCard()
        {
            if (_position != Position.CardBay)
            {
                _logger.LogError("Cannot replace card: not in card bay position.");
                return false;
            }

            const byte zIndex = 2;
            var coordinates = _coordinateStore.GetCoordinates();
            var replaceDistance = (uint)Math.Abs((long)coordinates.ReplaceDistance);
            var replaceDistanceSlow = (uint)Math.Abs((long)coordinates.ReplaceDistanceSlow);
            var startingPulseWidth = StaticConfig.Instance.StepperConfigs[zIndex].activeModeConfig.startingPulseWidth;

            if (!await stepperMoveActive(zIndex, true, replaceDistance))
            {
                _logger.LogError("Failed to move stepper {StepperIndex} forward by replacement distance {ReplaceDistance}.", zIndex, coordinates.ReplaceDistance);
                return false;
            }

            if (!await stepperSetConstActivePeriod(zIndex, startingPulseWidth))
            {
                _logger.LogError("Failed to set the slow active period for stepper {StepperIndex} while replacing card.", zIndex);
                return false;
            }

            if (!await stepperMoveActive(zIndex, true, replaceDistanceSlow))
            {
                _logger.LogError("Failed to move stepper {StepperIndex} forward by slow replacement distance {ReplaceDistanceSlow}.", zIndex, coordinates.ReplaceDistanceSlow);
                return false;
            }

            if (!await stepperSetDefaultActivePeriods(zIndex))
            {
                _logger.LogError("Failed to restore default active periods for stepper {StepperIndex} while replacing card.", zIndex);
                return false;
            }

            if (!await pliersOpen())
            {
                _logger.LogError("Failed to open pliers while replacing card.");
                return false;
            }

            await Task.Delay(200);

            var returnDistance = (uint)Math.Abs((long)coordinates.ReplaceDistance + coordinates.ReplaceDistanceSlow);
            if (!await stepperMoveActive(zIndex, false, returnDistance))
            {
                _logger.LogError("Failed to move stepper {StepperIndex} backward after replacing card.", zIndex);
                return false;
            }

            if (!await pliersDeactivate())
            {
                _logger.LogError("Failed to deactivate pliers after replacing card.");
                return false;
            }

            return true;
        }

        private async Task swipe(string cardName, uint opt)
        {
            try
            {
                var hasCard = false;
                byte bayIndex = 0;
                byte cardIndex = 0;
                if (_cardMappingStore.GetMappings().TryGetValue(cardName, out var slot) &&
                    slot.CardBay >= 0 && slot.CardBay < _cardSlotsOccupied.GetLength(0) &&
                    slot.CardSlotNumber >= 0 && slot.CardSlotNumber < _cardSlotsOccupied.GetLength(1))
                {
                    bayIndex = (byte)slot.CardBay;
                    cardIndex = (byte)slot.CardSlotNumber;
                    lock (_dataLock)
                    {
                        if (_cardSlotsOccupied[slot.CardBay, slot.CardSlotNumber])
                        {
                            _cardSlotsOccupied[slot.CardBay, slot.CardSlotNumber] = false;
                            hasCard = true;
                        }
                    }
                }

                if (!hasCard)
                {
                    _logger.LogError("Cannot swipe card {CardName}: no card is present in the slot.", cardName);
                    return;
                }

                if (!await toCardBay(bayIndex, cardIndex))
                {
                    _logger.LogError("Failed to move to card bay {BayIndex}, slot {CardIndex} while swiping card {CardName}.", bayIndex, cardIndex, cardName);
                    return;
                }

                if (!await fetchCard())
                {
                    _logger.LogError("Failed to fetch card {CardName} from card bay {BayIndex}, slot {CardIndex}.", cardName, bayIndex, cardIndex);
                    return;
                }
                
                if (!await toMsrSwiping(opt))
                {
                    _logger.LogError("Failed to move card {CardName} to the MSR swiping position using option {Option}.", cardName, opt);
                    return;
                }

                if (!await swipeCard(opt))
                {
                    _logger.LogError("Failed to swipe card {CardName} using option {Option}.", cardName, opt);
                    return;
                }

                if (!await toCardBay(bayIndex, cardIndex))
                {
                    _logger.LogError("Failed to move back to card bay {BayIndex}, slot {CardIndex} after swiping card {CardName}.", bayIndex, cardIndex, cardName);
                    return;
                }

                if (!await replaceCard())
                {
                    _logger.LogError("Failed to replace card {CardName} in card bay {BayIndex}, slot {CardIndex}.", cardName, bayIndex, cardIndex);
                    return;
                }

                {
                    lock (_dataLock)
                    {
                        _cardSlotsOccupied[bayIndex, cardIndex] = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to swipe card {CardName}.", cardName);
                return;
            }
            finally
            {
                lock (_dataLock)
                {
                    _action = Action.Idle;
                }
            }
        }

        public bool StartInsertingCard(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Inserting;
            }

            _ = Task.Run(() => insert(cardName, opt));
            return true;
        }

        private async Task insert(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartExtractingCard(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Extracting;
            }

            _ = Task.Run(() => extract(cardName, opt));
            return true;
        }

        private async Task extract(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartTappingCard(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Tapping;
            }

            _ = Task.Run(() => tap(cardName, opt));
            return true;
        }

        private async Task tap(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartBarCoding(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Barcoding;
            }

            _ = Task.Run(() => barCode(cardName, opt));
            return true;
        }

        private async Task barCode(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartTouching(uint opt, IReadOnlyList<(int x, int y)> coordinates)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Touching;
            }

            _ = Task.Run(() => touch(opt, coordinates));
            return true;
        }

        private async Task touch(uint opt, IReadOnlyList<(int x, int y)> coordinates)
        {
            await Task.CompletedTask;
        }
    }

    internal static class OperationsApi
    {
        private sealed record OperationRequest(string CardName, uint Opt);

        public static IEndpointRouteBuilder MapOperationsApi(this IEndpointRouteBuilder endpoints)
        {
            var api = endpoints.MapGroup("/api/operations");

            api.MapPost("/initialize", (Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartInitializingMachine() });
            });

            api.MapPost("/swipe", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartSwipingCard(request.CardName, request.Opt) });
            });

            api.MapPost("/tap", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartTappingCard(request.CardName, request.Opt) });
            });

            api.MapPost("/barcode", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartBarCoding(request.CardName, request.Opt) });
            });

            api.MapPost("/insert", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartInsertingCard(request.CardName, request.Opt) });
            });

            api.MapPost("/extract", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartExtractingCard(request.CardName, request.Opt) });
            });

            return endpoints;
        }
    }
}
