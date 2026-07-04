using System.Text.Json;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Configuration;
using VirtualOperatorServer.Facade;
using VirtualOperatorServer.Services;

internal sealed class ClientHandlers
{
    private const byte MaxAmountOfPulseInBatch = 100;
    private readonly BackSocket _backSocket;

    public ClientHandlers(BackSocket backSocket)
    {
        _backSocket = backSocket;
    }

    public IResult GetCommandHandler(string command)
    {
        if (command == "Status" || command.StartsWith("Status/", StringComparison.Ordinal))
        {
            return Results.Text(StatusFacade.CreateStatusJsonString(), "text/html");
        }

        Console.WriteLine($"Error: unknown GET command: {command}");
        return Results.Text("", "text/html");
    }

    public async Task<IResult> PostCommandHandler(string command, JsonElement jsonRoot)
    {
        try
        {
            if (command == "refreshStatus" ||
                command == "setGpio" ||
                command == "setPowerOutput" ||
                command == "setBDCPowerOutput" ||
                command == "setBDCControl" ||
                command == "disableStepper" ||
                command == "forwardStepper" ||
                command == "clockStepper" ||
                command == "setTimerPrescaler")
            {
                return await HandleMiscPostCommands(command, jsonRoot);
            }
            else if (command == "saveTimerPrescaler")
            {
                return SaveTimerPrescaler(jsonRoot);
            }
            else if (command == "saveStepperConfigTimer")
            {
                return SaveStepperConfigTimer(command, jsonRoot);
            }
            else if (command == "saveStepperConfigEncoder")
            {
                return SaveStepperConfigEncoder(command, jsonRoot);
            }
            else if (command == "saveStepperConfigMode")
            {
                return SaveStepperConfigMode(command, jsonRoot);
            }
            else if (command == "saveStepperConfig")
            {
                return SaveStepperConfig(command, jsonRoot);
            }
            else if (command == "setActivePeriods")
            {
                return await SetActivePeriods(jsonRoot);
            }
            else if (command == "setShortMoveActivePeriods")
            {
                return await SetShortMoveActivePeriods(jsonRoot);
            }
            else if (command == "setStepperControls")
            {
                return await SetStepperControls(jsonRoot);
            }
            else if (command == "setStepperEnable")
            {
                return await SetStepperEnable(jsonRoot);
            }
            else if (command == "setStepperForward")
            {
                return await SetStepperForward(jsonRoot);
            }
            else if (command == "startStepperHomePositioning")
            {
                return await StartStepperHomePositioning(jsonRoot);
            }
            else if (command == "setActiveSteps")
            {
                return await SetActiveSteps(jsonRoot);
            }
            else if (command == "runStepper")
            {
                return await RunStepper(jsonRoot);
            }
            else if (command == "testTimer")
            {
                return await TestTimer(jsonRoot);
            }
            else if (command == "testStepperEnable")
            {
                return await TestStepperEnable(jsonRoot);
            }
            else if (command == "testStepperForward")
            {
                return await TestStepperForward(jsonRoot);
            }
            else if (command == "testStepperClock")
            {
                return await TestStepperClock(jsonRoot);
            }
            else if (command == "testStepperForce")
            {
                return await TestStepperForce(jsonRoot);
            }
            else if (command == "testStepperPulseEnd")
            {
                return await TestStepperPulseEnd(jsonRoot);
            }
            else if (command == "testStepperStateReady")
            {
                return await TestStepperStateReady(jsonRoot);
            }
            else if (command == "testStepperActive")
            {
                return await TestStepperActive(jsonRoot);
            }
        }
        catch (InvalidRequestBodyException e)
        {
            Console.WriteLine($"Error: exception in buildPostCommand(): {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: exception in MapPost: {e.Message}");
        }

        return TypedResults.NotFound(new { error = "Item not found", command });
    }

    private async Task<string> RunCommand(CommandAndReply cmd)
    {
        if (cmd.Command.Length < 1)
        {
            throw new Exception("Invalid command length");
        }

        cmd.Reply = await _backSocket.SendAndReceiveAsync(cmd.Command);

        bool success;
        string reason;

        (success, reason) = cmd.ParseReply();
        if (success)
        {
            return "success";
        }
        else
        {
            return $"failure: {reason}";
        }
    }

    private async Task<IResult> RunStepperForced(byte stepperId, uint steps)
    {
        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new Exception($"Invalid stepper index '{stepperId}' in RunStepperForced");
        }
        if (steps > 1024)
        {
            throw new Exception($"Invalid steps '{steps}' in RunStepperForced");
        }

        var stepperConfig = StaticConfig.Instance.StepperConfigs[stepperId];

        if (stepperConfig.timer == StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
        {
            throw new Exception($"Timer must be selected in RunStepperForced");
        }

        ushort pulseWidth = stepperConfig.forcedModeConfig.pulseWidth;
        string result;

        var cmd = new CmdRunStepperForce(stepperId, (byte)stepperConfig.timer, pulseWidth, (ushort)steps);
        result = await RunCommand(cmd);
        if (result != "success")
        {
            return Results.Text($"failure: RunStepperForced: {result}", "text/html");
        }

        return Results.Text(result, "text/html");
    }

    private async Task<string> SetStepperCrossBoudnary(byte stepperId)
    {
        var stepperConfig = StaticConfig.Instance.StepperConfigs[stepperId];

        CmdSetStepperCrossBoundary.CrossBoudanry crossBoudanry = new CmdSetStepperCrossBoundary.CrossBoudanry();

        if (crossBoudanry.items.Length != stepperConfig.crossBoundary.boundaries.Length)
        {
            throw new Exception($"CrossBoudnary items account are different between command and configuration");
        }

        crossBoudanry.enabled = stepperConfig.crossBoundary.enabled;
        crossBoudanry.negativeRange = stepperConfig.crossBoundary.negativeRange;
        for (int i = 0; i < crossBoudanry.items.Length; i++)
        {
            crossBoudanry.items[i].enabled = stepperConfig.crossBoundary.boundaries[i].enabled;
            crossBoudanry.items[i].offset = stepperConfig.crossBoundary.boundaries[i].value;
            crossBoudanry.items[i].error = stepperConfig.crossBoundary.boundaries[i].error;
        }

        var cmdSetCrossBoundary = new CmdSetStepperCrossBoundary(stepperId, crossBoudanry);
        string result = await RunCommand(cmdSetCrossBoundary);

        if (result != "success")
        {
            return $"failure: CmdSetStepperCrossBoundary: {result}";
        }

        return "success";
    }

    private async Task<IResult> RunStepperActive(byte stepperId, uint steps)
    {
        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new Exception($"Invalid stepper index '{stepperId}' in RunStepperActive");
        }

        var stepperConfig = StaticConfig.Instance.StepperConfigs[stepperId];

        if (stepperConfig.timer == StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
        {
            throw new Exception($"Timer must be selected in RunStepperActive");
        }

        string result = await SetStepperCrossBoudnary(stepperId);
        if (result != "success")
        {
            return Results.Text($"{result}", "text/html");
        }

        var cmdSetActive = new CmdSetStepperActive(stepperId, steps);
        result = await RunCommand(cmdSetActive);
        if (result != "success")
        {
            return Results.Text($"failure: CmdSetStepperActive: {result}", "text/html");
        }

        var cmdRunActive = new CmdRunStepperActive(stepperId, (byte)stepperConfig.timer);
        result = await RunCommand(cmdRunActive);
        if (result != "success")
        {
            return Results.Text($"failure: RunStepperActive: {result}", "text/html");
        }

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> RunStepperPassive(byte stepperId, uint steps, byte activeStepperId, uint activeSteps)
    {

        if (activeSteps >= ushort.MaxValue)
        {
            return Results.Text($"failure: RunStepperPassive: too many active steps: {activeSteps}", "text/html");
        }

        string result;

        result = await SetStepperCrossBoudnary(stepperId);
        if (result != "success")
        {
            return Results.Text($"{result}", "text/html");
        }

        // calculate passive indexes
        var indexes = new ushort[steps];

        if (steps == 1)
        {
            indexes[0] = (ushort)(activeSteps / 2);
        }
        else if (steps == activeSteps)
        {
            for (uint i = 0; i < steps; i++)
            {
                indexes[i] = (ushort)i;
            }
        }
        else
        {
            double ratio = (double)activeSteps / (double)steps;

            for (uint i = 0; i < steps; i++)
            {
                // consider these situations:
                //     activeSteps == 10, steps == 3
                //     activeSteps == 10, steps == 5

                ushort index = (ushort)Math.Floor((i + 1) * ratio);
                if (index == (i + 1) * ratio)
                {
                    index -= 1;
                }

                indexes[i] = index;
            }
        }

        // transfer passive indexes 
        var indexList = new List<ushort>();
        const byte indexesPerBatch = 200;
        byte batchIndex = 0;
        byte totalBatches = (byte)(indexes.Length / indexesPerBatch + 1);
        CommandAndReply cmd;

        for (uint i = 0; i < indexes.Length; i++)
        {
            indexList.Add(indexes[i]);
            if (indexList.Count == indexesPerBatch)
            {
                cmd = new CmdSetStepperPassiveStepIndexes(stepperId, batchIndex, totalBatches, indexList.ToArray());
                result = await RunCommand(cmd);
                if (result != "success")
                {
                    return Results.Text($"failure: RunStepperPassive: failed to set passive steps: batchIndex: {batchIndex}, totalBatches: {totalBatches}, result: {result}", "text/html");
                }

                indexList.Clear();
                batchIndex++;
            }
        }
        if (indexList.Count > 0)
        {
            cmd = new CmdSetStepperPassiveStepIndexes(stepperId, batchIndex, totalBatches, indexList.ToArray());
            result = await RunCommand(cmd);
            if (result != "success")
            {
                return Results.Text($"failure: RunStepperPassive: failed to set passive steps: batchIndex: {batchIndex}, totalBatches: {totalBatches}, result: {result}", "text/html");
            }
        }

        // couple active and passive steppers
        cmd = new CmdRunStepperPasive(stepperId, activeStepperId);
        result = await RunCommand(cmd);
        if (result != "success")
        {
            return Results.Text($"failure: RunStepperPassive: failed to couple active stepper: {result}", "text/html");
        }

        var activeStepperConfig = StaticConfig.Instance.StepperConfigs[activeStepperId];
        if (activeStepperConfig.timer == StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
        {
            throw new Exception($"Timer in active stepper must be selected in RunStepperPassive");
        }

        // start clocking active stepper
        cmd = new CmdRunStepperActive(activeStepperId, (byte)activeStepperConfig.timer);
        result = await RunCommand(cmd);
        if (result != "success")
        {
            return Results.Text($"failure: RunStepperPassive: failed to couple active stepper: {result}", "text/html");
        }

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> RunStepper(JsonElement payload)
    {
        var stepperId = payload.GetProperty("stepperId").GetByte();
        var mode = payload.GetProperty("mode").GetString();
        var steps = payload.GetProperty("steps").GetUInt32();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            return Results.Text($"failure: invalid stepperId '{stepperId}'", "text/html");
        }
        if (steps == 0)
        {
            return Results.Text($"failure: invalid steps '{steps}'", "text/html");
        }

        switch (mode)
        {
            case "forced":
                return await RunStepperForced(stepperId, steps);
            case "active":
                return await RunStepperActive(stepperId, steps);
            case "passive":
                {
                    byte activeStepperId = payload.GetProperty("activeStepperId").GetByte();

                    if ((activeStepperId >= StatusFacade.Facade.StepperCount) ||
                        (activeStepperId == stepperId))
                    {
                        return Results.Text($"failure: invalid activeStepperId '{activeStepperId}'", "text/html");
                    }

                    uint activeSteps = payload.GetProperty("activeSteps").GetUInt32();
                    if (steps > activeSteps)
                    {
                        return Results.Text($"failure: invalid steps '{steps}'", "text/html");
                    }

                    return await RunStepperPassive(stepperId, steps, activeStepperId, activeSteps);
                }
            default:
                return Results.Text($"failure: unknown mode '{mode}'", "text/html");
        }
    }


    private async Task<IResult> HandleMiscPostCommands(string command, JsonElement jsonRoot)
    {
        CommandAndReply cmd = CommandFactory.BuildPostCommand(command, jsonRoot);

        var result = await RunCommand(cmd);
        return Results.Text(result, "text/html"); ;
    }

    private static IResult SaveTimerPrescaler(JsonElement jsonRoot)
    {
        var timerId = jsonRoot.GetProperty("timerId").GetByte();
        var prescaler = jsonRoot.GetProperty("prescaler").GetUInt16();
        var timerConfigs = StaticConfig.Instance.TimerConfigs;

        if (timerId >= timerConfigs.Length)
        {
            throw new InvalidRequestBodyException($"invalid timer id: {timerId}");
        }

        timerConfigs[timerId] = prescaler;
        StaticConfig.Instance.SaveTimerConfigs();

        return Results.Text("success", "text/html"); ;
    }

    private static IResult SaveStepperConfigTimer(string command, JsonElement jsonRoot)
    {
        var stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        var timer = jsonRoot.GetProperty("timer").GetString();

        StatusFacade.Facade.Stepper.Configuration.EnumTimer enumTimer;

        switch (timer)
        {
            case "FLEX_TIMER_0":
                enumTimer = StatusFacade.Facade.Stepper.Configuration.EnumTimer.FLEX_TIMER_0;
                break;
            case "FLEX_TIMER_1":
                enumTimer = StatusFacade.Facade.Stepper.Configuration.EnumTimer.FLEX_TIMER_1;
                break;
            case "FLEX_TIMER_2":
                enumTimer = StatusFacade.Facade.Stepper.Configuration.EnumTimer.FLEX_TIMER_2;
                break;
            case "FLEX_TIMER_3":
                enumTimer = StatusFacade.Facade.Stepper.Configuration.EnumTimer.FLEX_TIMER_3;
                break;
            case "FLEX_TIMER_4":
                enumTimer = StatusFacade.Facade.Stepper.Configuration.EnumTimer.FLEX_TIMER_4;
                break;
            case "FLEX_TIMER_5":
                enumTimer = StatusFacade.Facade.Stepper.Configuration.EnumTimer.FLEX_TIMER_5;
                break;
            case "FIX_TIMER":
                enumTimer = StatusFacade.Facade.Stepper.Configuration.EnumTimer.FIX_TIMER;
                break;
            default:
                throw new Exception($"Invalid timer '{timer}' in POST command '{command}'");
        }

        var configs = StaticConfig.Instance.StepperConfigs;
        if (stepperIndex >= configs.Length)
        {
            throw new Exception($"Invalid stepper index '{stepperIndex}' in POST command '{command}'");
        }

        configs[stepperIndex].timer = enumTimer;
        StaticConfig.Instance.SaveStepperConfigs();

        return Results.Text("success", "text/html");
    }

    private static IResult SaveStepperConfigEncoder(string command, JsonElement jsonRoot)
    {
        var stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        var encoder = jsonRoot.GetProperty("encoder").GetString();

        StatusFacade.Facade.Stepper.Configuration.EnumEncoder enumEncoder;

        switch (encoder)
        {
            case "ENC0":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.ENC0;
                break;
            case "ENC1":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.ENC1;
                break;
            case "ENC2":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.ENC2;
                break;
            case "ENC3":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.ENC3;
                break;
            case "ENC4":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.ENC4;
                break;
            case "ENC5":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.ENC5;
                break;
            case "ENC6":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.ENC6;
                break;
            case "ENC7":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.ENC7;
                break;
            case "NOT_SELECTED":
                enumEncoder = StatusFacade.Facade.Stepper.Configuration.EnumEncoder.NOT_SELECTED;
                break;
            default:
                throw new Exception($"Invalid encoder '{encoder}' in POST command '{command}'");
        }

        var configs = StaticConfig.Instance.StepperConfigs;
        if (stepperIndex >= configs.Length)
        {
            throw new Exception($"Invalid stepper index '{stepperIndex}' in POST command '{command}'");
        }

        configs[stepperIndex].encoder = enumEncoder;
        StaticConfig.Instance.SaveStepperConfigs();

        return Results.Text("success", "text/html");
    }

    private static IResult SaveStepperConfigMode(string command, JsonElement jsonRoot)
    {
        var stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        var mode = jsonRoot.GetProperty("mode").GetString();
        var configs = StaticConfig.Instance.StepperConfigs;

        if (stepperIndex >= configs.Length)
        {
            throw new Exception($"Invalid stepper index '{stepperIndex}' in POST command '{command}'");
        }

        switch (mode)
        {
            case "forced":
                {
                    var value = jsonRoot.GetProperty("value").GetUInt16();
                    configs[stepperIndex].forcedModeConfig.pulseWidth = value;
                    break;
                }
            case "active":
                {
                    var type = jsonRoot.GetProperty("type").GetString();
                    var value = jsonRoot.GetProperty("value").GetUInt16();

                    switch (type)
                    {
                        case "starting":
                            configs[stepperIndex].activeModeConfig.startingPulseWidth = value;
                            break;
                        case "accelerationSteps":
                            configs[stepperIndex].activeModeConfig.acceleratingSteps = value;
                            break;
                        case "cruising":
                            configs[stepperIndex].activeModeConfig.cruisingPulseWidth = value;
                            break;
                        case "ending":
                            configs[stepperIndex].activeModeConfig.endingPulseWidth = value;
                            break;
                        case "deaccelerationSteps":
                            configs[stepperIndex].activeModeConfig.deacceleratingSteps = value;
                            break;
                        default:
                            throw new Exception($"Invalid type '{type}' in POST command '{command}'");
                    }
                    break;
                }
            default:
                throw new Exception($"Invalid mode '{mode}' in POST command '{command}'");
        }

        StaticConfig.Instance.SaveStepperConfigs();
        return Results.Text("success", "text/html");
    }

    private static IResult SaveStepperConfig(string command, JsonElement jsonRoot)
    {
        var stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        var classification = jsonRoot.GetProperty("classification").GetString();
        var configs = StaticConfig.Instance.StepperConfigs;

        if (stepperIndex >= configs.Length)
        {
            throw new Exception($"Invalid stepper index '{stepperIndex}' in POST command '{command}'");
        }

        switch (classification)
        {
            case "isEnableHigh":
                {
                    var isChecked = jsonRoot.GetProperty("isChecked").GetBoolean();
                    configs[stepperIndex].isEnableHigh = isChecked;
                }
                break;
            case "isForwardHigh":
                {
                    var isChecked = jsonRoot.GetProperty("isChecked").GetBoolean();
                    configs[stepperIndex].isForwardHigh = isChecked;
                }
                break;
            case "isRisingEdgeDriven":
                {
                    var isChecked = jsonRoot.GetProperty("isChecked").GetBoolean();
                    configs[stepperIndex].isRisingEdgeDriven = isChecked;
                }
                break;
            case "homeBoundaryToReadySteps":
                {
                    var value = jsonRoot.GetProperty("value").GetUInt32();
                    configs[stepperIndex].homeBoundaryToReadySteps = (ushort)value;
                }
                break;
            case "range":
                {
                    var value = jsonRoot.GetProperty("value").GetUInt32();
                    configs[stepperIndex].range = value;
                }
                break;
            case "stepsPerRotation":
                {
                    var value = jsonRoot.GetProperty("value").GetUInt32();
                    configs[stepperIndex].stepsPerRotation = (ushort)value;
                }
                break;
            case "encoderCountsPerRotation":
                {
                    var value = jsonRoot.GetProperty("value").GetUInt32();
                    configs[stepperIndex].encoderCountsPerRotation = (ushort)value;
                }
                break;
            case "encoderOffsetErrorThreshold":
                {
                    var value = jsonRoot.GetProperty("value").GetUInt32();
                    configs[stepperIndex].encoderOffsetErrorThreshold = (ushort)value;
                }
                break;
            case "port":
                {
                    var value = jsonRoot.GetProperty("value").GetString();
                    if (Enum.TryParse(value, out StatusFacade.Facade.Stepper.Configuration.EnumPort enumPort))
                    {
                        var name = jsonRoot.GetProperty("name").GetString();
                        switch (name)
                        {
                            case "HomeBoundary":
                                configs[stepperIndex].portHomeBoundary = enumPort;
                                break;
                            case "EndBoundary":
                                configs[stepperIndex].portEndBoundary = enumPort;
                                break;
                            case "Enable":
                                configs[stepperIndex].portEnable = enumPort;
                                break;
                            case "Forward":
                                configs[stepperIndex].portForward = enumPort;
                                break;
                            case "Clock":
                                configs[stepperIndex].portClock = enumPort;
                                break;
                            default:
                                throw new Exception($"Invalid port name '{name}' in POST command '{command}'");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid port '{value}' in POST command '{command}'");
                    }
                }
                break;
            case "pin":
                {
                    var value = jsonRoot.GetProperty("value").GetString();
                    if (byte.TryParse(value, out byte pin))
                    {
                        var name = jsonRoot.GetProperty("name").GetString();
                        switch (name)
                        {
                            case "HomeBoundary":
                                configs[stepperIndex].pinHomeBoundary = pin;
                                break;
                            case "EndBoundary":
                                configs[stepperIndex].pinEndBoundary = pin;
                                break;
                            case "Enable":
                                configs[stepperIndex].pinEnable = pin;
                                break;
                            case "Forward":
                                configs[stepperIndex].pinForward = pin;
                                break;
                            case "Clock":
                                configs[stepperIndex].pinClock = pin;
                                break;
                            default:
                                throw new Exception($"Invalid pin name '{name}' in POST command '{command}'");
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid port '{value}' in POST command '{command}'");
                    }
                }
                break;
            case "crossBoundary":
                {
                    var isEnabled = jsonRoot.GetProperty("enabled").GetBoolean();
                    var negativeRange = jsonRoot.GetProperty("negativeRange").GetInt32();

                    configs[stepperIndex].crossBoundary.enabled = isEnabled;
                    configs[stepperIndex].crossBoundary.negativeRange = negativeRange;
                }
                break;
            case "crossBoundaryItem":
                {
                    var itemIndex = jsonRoot.GetProperty("itemIndex").GetUInt32();
                    var isEnabled = jsonRoot.GetProperty("isEnabled").GetBoolean();
                    var boundaryValue = jsonRoot.GetProperty("boundaryValue").GetInt32();
                    var boundaryError = jsonRoot.GetProperty("boundaryError").GetUInt16();

                    var boundaryItem = configs[stepperIndex].crossBoundary.boundaries[itemIndex];
                    boundaryItem.enabled = isEnabled;
                    boundaryItem.value = boundaryValue;
                    boundaryItem.error = boundaryError;
                }
                break;
            default:
                throw new Exception($"Invalid classification '{classification}' in POST command '{command}'");
        }

        StaticConfig.Instance.SaveStepperConfigs();
        return Results.Text("success", "text/html");
    }

    private async Task<IResult> SetActivePeriods(JsonElement jsonRoot)
    {
        byte stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        var configs = StaticConfig.Instance.StepperConfigs;
        var status = CmdGetStatus.Status;

        if (stepperIndex >= configs.Length)
        {
            throw new InvalidRequestBodyException($"Invalid stepper index '{stepperIndex}'");
        }
        if (status == null)
        {
            throw new InvalidRequestBodyException("CmdGetStatus.Status is not ready");
        }

        var config = configs[stepperIndex];

        var timerId = config.timer;
        if (timerId == StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
        {
            throw new InvalidRequestBodyException($"Timer is not selected for stepper: {stepperIndex}");
        }

        uint timerClockPeriodNs;
        if (timerId == StatusFacade.Facade.Stepper.Configuration.EnumTimer.FIX_TIMER)
        {
            timerClockPeriodNs = status.timersData[(int)timerId].prescaler * (uint)17;
        }
        else
        {
            timerClockPeriodNs = (uint)(status.timersData[(int)timerId].prescaler * 4.16);
        }

        if (config.activeModeConfig.acceleratingSteps < 1)
        {
            throw new InvalidRequestBodyException($"Invalid count of rampup periods: {config.activeModeConfig.acceleratingSteps}");
        }
        else
        {
            IStepperPulses stepsBuilder;
            try
            {
                stepsBuilder = StepperPulsesFactory.Create(config.activeModeConfig.startingPulseWidth,
                                                           config.activeModeConfig.acceleratingSteps,
                                                           config.activeModeConfig.cruisingPulseWidth,
                                                           timerClockPeriodNs);
            }
            catch (Exception ex)
            {
                throw new InvalidRequestBodyException($"Invalid rampup pulse configuration: {ex.Message}");
            }

            var result = await SendActiveAccelerationPeriods(stepperIndex, stepsBuilder.SCurvePulsesRampup);
            if (result != null)
            {
                return result;
            }
        }

        {
            var result = await SendActiveCruisingPeriod(stepperIndex, config.activeModeConfig.cruisingPulseWidth);
            if (result != null)
            {
                return result;
            }
        }

        if (config.activeModeConfig.deacceleratingSteps < 1)
        {
            throw new InvalidRequestBodyException($"Invalid count of rampdown periods: {config.activeModeConfig.deacceleratingSteps}");
        }
        else
        {
            IStepperPulses stepsBuilder;
            try
            {
                stepsBuilder = StepperPulsesFactory.Create(config.activeModeConfig.endingPulseWidth,
                                                           config.activeModeConfig.deacceleratingSteps,
                                                           config.activeModeConfig.cruisingPulseWidth,
                                                           timerClockPeriodNs);
            }
            catch (Exception ex)
            {
                throw new InvalidRequestBodyException($"Invalid rampdown pulse configuration: {ex.Message}");
            }

            var result = await SendActiveDeaccelerationPeriods(stepperIndex, stepsBuilder.SCurvePulsesRampdown);
            if (result != null)
            {
                return result;
            }
        }

        return Results.Text("success", "text/html");
    }

    private async Task<IResult> SetShortMoveActivePeriods(JsonElement jsonRoot)
    {
        byte stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        int steps = jsonRoot.GetProperty("steps").GetInt32();
        var configs = StaticConfig.Instance.StepperConfigs;
        var status = CmdGetStatus.Status;

        if (stepperIndex >= configs.Length)
        {
            throw new InvalidRequestBodyException($"Invalid stepper index '{stepperIndex}'");
        }
        if (steps < 1)
        {
            throw new InvalidRequestBodyException($"Invalid short move steps: {steps}");
        }
        if (status == null)
        {
            throw new InvalidRequestBodyException("CmdGetStatus.Status is not ready");
        }

        var config = configs[stepperIndex];

        var timerId = config.timer;
        if (timerId == StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
        {
            throw new InvalidRequestBodyException($"Timer is not selected for stepper: {stepperIndex}");
        }

        uint timerClockPeriodNs;
        if (timerId == StatusFacade.Facade.Stepper.Configuration.EnumTimer.FIX_TIMER)
        {
            timerClockPeriodNs = status.timersData[(int)timerId].prescaler * (uint)17;
        }
        else
        {
            timerClockPeriodNs = (uint)(status.timersData[(int)timerId].prescaler * 4.16);
        }

        if (config.activeModeConfig.acceleratingSteps < 1)
        {
            throw new InvalidRequestBodyException($"Invalid count of rampup periods: {config.activeModeConfig.acceleratingSteps}");
        }

        IStepperPulses stepsBuilder;
        try
        {
            stepsBuilder = StepperPulsesFactory.Create(config.activeModeConfig.startingPulseWidth,
                                                       config.activeModeConfig.acceleratingSteps,
                                                       config.activeModeConfig.cruisingPulseWidth,
                                                       timerClockPeriodNs);
        }
        catch (Exception ex)
        {
            throw new InvalidRequestBodyException($"Invalid short move pulse configuration: {ex.Message}");
        }

        var shortDistancePulses = stepsBuilder.GetShortDistancePulses(steps);
        if (shortDistancePulses == null)
        {
            throw new InvalidRequestBodyException($"Invalid short move steps: {steps}");
        }

        var accelerationStepCount = shortDistancePulses.Count / 2;
        var hasCruisingStep = (shortDistancePulses.Count % 2) == 1;
        var deaccelerationStartIndex = accelerationStepCount + (hasCruisingStep ? 1 : 0);

        ushort[] acceleratingPeriods = new ushort[accelerationStepCount];
        for (int i = 0; i < acceleratingPeriods.Length; i++)
        {
            acceleratingPeriods[i] = shortDistancePulses[i];
        }

        var cruisingPulseWidth = shortDistancePulses[Math.Min(accelerationStepCount, shortDistancePulses.Count - 1)];

        ushort[] deacceleratingPeriods = new ushort[shortDistancePulses.Count - deaccelerationStartIndex];
        for (int i = 0; i < deacceleratingPeriods.Length; i++)
        {
            deacceleratingPeriods[i] = shortDistancePulses[deaccelerationStartIndex + i];
        }

        var result = await SendActiveAccelerationPeriods(stepperIndex, acceleratingPeriods);
        if (result != null)
        {
            return result;
        }

        result = await SendActiveCruisingPeriod(stepperIndex, cruisingPulseWidth);
        if (result != null)
        {
            return result;
        }

        result = await SendActiveDeaccelerationPeriods(stepperIndex, deacceleratingPeriods);
        if (result != null)
        {
            return result;
        }

        return Results.Text("success", "text/html");
    }

    private async Task<IResult?> SendActiveAccelerationPeriods(byte stepperIndex,
                                                                      IReadOnlyList<ushort> acceleratingPeriods,
                                                                      BackSocket backSocket)
    {
        var totalBatches = (acceleratingPeriods.Count + MaxAmountOfPulseInBatch - 1) / MaxAmountOfPulseInBatch;
        for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
        {
            ushort[] batch;

            if ((batchIndex + 1) * MaxAmountOfPulseInBatch > acceleratingPeriods.Count)
            {
                batch = new ushort[acceleratingPeriods.Count - batchIndex * MaxAmountOfPulseInBatch];
            }
            else
            {
                batch = new ushort[MaxAmountOfPulseInBatch];
            }

            for (int i = 0; i < batch.Length; i++)
            {
                batch[i] = acceleratingPeriods[batchIndex * MaxAmountOfPulseInBatch + i];
            }

            var cmd = new CmdSetStepperActiveRampupPulseWidth(stepperIndex, (byte)batchIndex, (byte)totalBatches, batch);
            var result = await RunCommand(cmd);
            if (result != "success")
            {
                return Results.Text($"Failed in set rampup periods: '{result}'", "text/html");
            }
        }

        return null;
    }

    private async Task<IResult?> SendActiveCruisingPeriod(byte stepperIndex,
                                                                 ushort cruisingPulseWidth,
                                                                 BackSocket backSocket)
    {
        var cmd = new CmdSetStepperActiveCruisePulseWidth(stepperIndex, cruisingPulseWidth);
        var result = await RunCommand(cmd);
        if (result != "success")
        {
            return Results.Text($"Failed in set cruising period: '{result}'", "text/html");
        }

        return null;
    }

    private async Task<IResult?> SendActiveDeaccelerationPeriods(byte stepperIndex,
                                                                        IReadOnlyList<ushort> deacceleratingPeriods,
                                                                        BackSocket backSocket)
    {
        var totalBatches = (deacceleratingPeriods.Count + MaxAmountOfPulseInBatch - 1) / MaxAmountOfPulseInBatch;
        for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
        {
            ushort[] batch;

            if ((batchIndex + 1) * MaxAmountOfPulseInBatch > deacceleratingPeriods.Count)
            {
                batch = new ushort[deacceleratingPeriods.Count - batchIndex * MaxAmountOfPulseInBatch];
            }
            else
            {
                batch = new ushort[MaxAmountOfPulseInBatch];
            }

            for (int i = 0; i < batch.Length; i++)
            {
                batch[i] = deacceleratingPeriods[batchIndex * MaxAmountOfPulseInBatch + i];
            }

            var cmd = new CmdSetStepperActiveRampdownPulseWidth(stepperIndex, (byte)batchIndex, (byte)totalBatches, batch);
            var result = await RunCommand(cmd);
            if (result != "success")
            {
                return Results.Text($"Failed in set rampdown periods: '{result}'", "text/html");
            }
        }

        return null;
    }

    private async Task<IResult> SetStepperControls(JsonElement jsonRoot)
    {
        byte stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        var configs = StaticConfig.Instance.StepperConfigs;

        if (stepperIndex >= configs.Length)
        {
            throw new InvalidRequestBodyException($"Invalid stepper index '{stepperIndex}'");
        }

        var config = configs[stepperIndex];
        var cmd = new CmdSetStepperControls(stepperIndex,
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
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> SetStepperEnable(JsonElement jsonRoot)
    {
        byte stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        bool isEnable = jsonRoot.GetProperty("enable").GetBoolean();

        if (stepperIndex >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper index '{stepperIndex}'");
        }

        var cmd = new CmdSetStepperEnable(stepperIndex, isEnable);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> SetStepperForward(JsonElement jsonRoot)
    {
        byte stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
        bool isForward = jsonRoot.GetProperty("forward").GetBoolean();

        if (stepperIndex >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper index '{stepperIndex}'");
        }

        var cmd = new CmdSetStepperForward(stepperIndex, isForward);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> StartStepperHomePositioning(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper index '{stepperId}'");
        }

        var stepperConfig = StaticConfig.Instance.StepperConfigs[stepperId];

        if (stepperConfig.timer == StatusFacade.Facade.Stepper.Configuration.EnumTimer.NOT_SELECTED)
        {
            throw new Exception($"Timer must be selected in startStepperHomePositioning");
        }

        var cmd = new CmdStartStepperHomePositioning(stepperId, (byte)stepperConfig.timer);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> SetActiveSteps(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();
        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper index '{stepperId}'");
        }

        uint steps = jsonRoot.GetProperty("steps").GetUInt32();

        var cmd = new CmdSetStepperActive(stepperId, steps);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> TestTimer(JsonElement jsonRoot)
    {
        byte timerId = jsonRoot.GetProperty("timerId").GetByte();
        ushort pulseWidth = jsonRoot.GetProperty("pulseWidth").GetUInt16();
        ushort totalPulse = jsonRoot.GetProperty("totalPulse").GetUInt16();
        ushort logPeriod = jsonRoot.GetProperty("logPeriod").GetUInt16();

        if (timerId >= (StatusFacade.Facade.FlexTimerCount + 1))
        {
            throw new InvalidRequestBodyException($"Invalid timer Id '{timerId}'");
        }

        var cmd = new CmdTestTimer(timerId, pulseWidth, totalPulse, logPeriod);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> TestStepperEnable(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();
        bool isEnable = jsonRoot.GetProperty("isEnable").GetBoolean();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper Id '{stepperId}'");
        }

        var cmd = new CmdTestStepperSignalEnable(stepperId, isEnable);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> TestStepperForward(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();
        bool isForward = jsonRoot.GetProperty("isForward").GetBoolean();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper Id '{stepperId}'");
        }

        var cmd = new CmdTestStepperSignalForward(stepperId, isForward);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> TestStepperClock(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();
        bool isFirstHalf = jsonRoot.GetProperty("isFirstHalf").GetBoolean();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper Id '{stepperId}'");
        }

        var cmd = new CmdTestStepperSignalClock(stepperId, isFirstHalf);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> TestStepperForce(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();
        ushort pulseWidth = jsonRoot.GetProperty("pulseWidth").GetUInt16();
        ushort steps = jsonRoot.GetProperty("steps").GetUInt16();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper Id '{stepperId}'");
        }

        var cmd = new CmdTestStepperStateRunningForce(stepperId, pulseWidth, steps);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> TestStepperPulseEnd(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper Id '{stepperId}'");
        }

        var cmd = new CmdTestStepperPulseEnd(stepperId);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> TestStepperStateReady(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper Id '{stepperId}'");
        }

        var cmd = new CmdTestStepperStateReady(stepperId);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

    private async Task<IResult> TestStepperActive(JsonElement jsonRoot)
    {
        byte stepperId = jsonRoot.GetProperty("stepperId").GetByte();
        uint steps = jsonRoot.GetProperty("steps").GetUInt32();

        if (stepperId >= StatusFacade.Facade.StepperCount)
        {
            throw new InvalidRequestBodyException($"Invalid stepper Id '{stepperId}'");
        }

        var cmd = new CmdTestStepperStateRunningActive(stepperId, steps);
        string result = await RunCommand(cmd);

        return Results.Text(result, "text/html");
    }

}
