
using System.Text.Json;
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
        private Action _action = Action.Idle;
        private Position _position = Position.Unknown;

        public Operations(ILogger<Operations> logger, BackSocket backSocket, CoordinateStore coordinateStore)
        {
            _logger = logger;
            _backSocket = backSocket;
            _coordinateStore = coordinateStore;
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

                var negativeRange = StaticConfig.Instance.StepperConfigs[clickerIndex].crossBoundary.negativeRange;
                if (!await stepperForward(clickerIndex, negativeRange >= 0))
                {
                    _logger.LogError("Failed to set direction for Clicker stepper during initialization.");
                    return false;
                }

                if (!await stepperRunActive(clickerIndex, (uint)Math.Abs((long)negativeRange)))
                {
                    _logger.LogError("Failed to move Clicker stepper to its negative range {NegativeRange} during initialization.", negativeRange);
                    return false;
                }

                while (true)
                {
                    await Task.Delay(200);

                    var (result, state) = await stepperGetStatus(clickerIndex);
                    if (!result)
                    {
                        _logger.LogError("Failed to retrieve the state of the Clicker stepper while moving to its negative range.");
                        return false;
                    }

                    if (state == Facade.StatusFacade.Facade.Stepper.Status.State.READY)
                        break;

                    if (state != Facade.StatusFacade.Facade.Stepper.Status.State.RUNNING_ACTIVE)
                    {
                        _logger.LogError("Clicker stepper entered unexpected state {StepperState} while moving to its negative range.", state);
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

                await Task.Delay(1000);

                if (!await initializeStepper())
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

        private async Task swipe(string cardName, uint opt)
        {
            await Task.CompletedTask;
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
