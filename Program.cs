using System.Diagnostics;
using System.Text.Json;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Facade;
using VirtualOperatorServer.Services;
using VirtualOperatorServer.Configuration;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BackSocket>();
builder.Services.AddHostedService<BackService>();

var app = builder.Build();
var backSocket = app.Services.GetRequiredService<BackSocket>();

app.UseDefaultFiles();
app.UseStaticFiles();

async Task<string> RunCommand(CommandAndReply cmd)
{
    if (cmd.Command.Length < 1)
    {
        throw new Exception("Invalid command length");
    }

    cmd.Reply = await backSocket.SendAndReceiveAsync(cmd.Command);

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

string ProcessGetCommand(string command)
{
    CommandAndReply cmd = new([]);

    string[] cmdSegments = command.Split('/');

    if (cmdSegments[0] == "Status")
    {
        return StatusFacade.CreateStatusJsonString();
    }
    else
    {
        Console.WriteLine($"Error: unknown GET command: {command}");
    }

    return "";
}

async Task<IResult> RunStepperForced(byte stepperId, uint steps)
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

async Task<IResult> RunStepperActive(byte stepperId, uint steps)
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

    var cmdSetActive = new CmdSetStepperActive(stepperId, steps);
    string result = await RunCommand(cmdSetActive);
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

async Task<IResult> RunStepperPassive(byte stepperId, uint steps, byte activeStepperId, uint activeSteps)
{

    if (activeSteps >= ushort.MaxValue)
    {
        return Results.Text($"failure: RunStepperPassive: too many active steps: {activeSteps}", "text/html");
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
    string result;

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

async Task<IResult> RunStepper(JsonElement payload)
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

app.MapGet("/get/{*command}", (string command) =>
{
    Console.WriteLine($"/get/{command}");
    if(command.Length == 0)
    {
        return Results.Text("Empty command", "text/html");
    }

    var reply = ProcessGetCommand(command);

    return Results.Text(reply, "text/html");
});

app.MapPost("/post/{*command}", async (HttpRequest request, string command, BackSocket backSocket) =>
{
    Console.WriteLine($"/post/{command}");
    if(command.Length == 0)
    {
        return Results.Text("Empty command", "text/html");
    }

    using var jsonDoc = await JsonDocument.ParseAsync(request.Body);
    var jsonRoot = jsonDoc.RootElement;

    try
    {
        if (
            (command == "refreshStatus") ||
            (command == "setGpio") ||
            (command == "setPowerOutput") ||
            (command == "setBDCPowerOutput") ||
            (command == "setBDCControl") ||
            (command == "disableStepper") ||
            (command == "forwardStepper") ||
            (command == "clockStepper") ||
            (command == "setTimerPrescaler")
        )
        {
            CommandAndReply cmd = CommandFactory.BuildPostCommand(command, jsonRoot);

            var result = await RunCommand(cmd);
            return Results.Text(result, "text/html"); ;
        }
        else if (command == "saveTimerPrescaler")
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
        else if (command == "saveStepperConfigTimer")
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
        else if (command == "saveStepperConfigEncoder")
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
        else if (command == "saveStepperConfigMode")
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
        else if (command == "saveStepperConfig")
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
        else if (command == "setActivePeriods")
        {
            const byte PERIODS_PER_BATCH = 100;
            byte stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
            var configs = StaticConfig.Instance.StepperConfigs;

            if (stepperIndex >= configs.Length)
            {
                throw new InvalidRequestBodyException($"Invalid stepper index '{stepperIndex}'");
            }

            var config = configs[stepperIndex];

            if (config.activeModeConfig.acceleratingSteps < 1)
            {
                throw new InvalidRequestBodyException($"Invalid count of rampup periods: {config.activeModeConfig.acceleratingSteps}");
            }
            else
            {
                ushort[] acceleratingPeriods = new ushort[config.activeModeConfig.acceleratingSteps];
                double acceleratingRate = config.activeModeConfig.startingPulseWidth - config.activeModeConfig.cruisingPulseWidth;
                acceleratingRate /= config.activeModeConfig.acceleratingSteps;
                if (acceleratingRate < 0)
                {
                    throw new InvalidRequestBodyException($"Invalid rampup periods: '{config.activeModeConfig.startingPulseWidth}', '{config.activeModeConfig.cruisingPulseWidth}'");
                }

                for (int i = 0; i < acceleratingPeriods.Length; i++)
                {
                    acceleratingPeriods[i] = (ushort)Math.Round(config.activeModeConfig.startingPulseWidth - i * acceleratingRate);
                }

                var totalBatches = (acceleratingPeriods.Length + PERIODS_PER_BATCH - 1) / PERIODS_PER_BATCH;
                for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    ushort[] batch;

                    if ((batchIndex + 1) * PERIODS_PER_BATCH > acceleratingPeriods.Length)
                    {
                        batch = new ushort[acceleratingPeriods.Length - batchIndex * PERIODS_PER_BATCH];
                    }
                    else
                    {
                        batch = new ushort[PERIODS_PER_BATCH];
                    }

                    for (int i = 0; i < batch.Length; i++)
                    {
                        batch[i] = acceleratingPeriods[batchIndex * PERIODS_PER_BATCH + i];
                    }

                    var cmd = new CmdSetStepperActiveRampupPulseWidth(stepperIndex, (byte)batchIndex, (byte)totalBatches, batch);
                    var result = await RunCommand(cmd);
                    if (result != "success")
                    {
                        return Results.Text($"Failed in set rampup periods: '{result}'", "text/html");
                    }
                }
            }

            // cruising period
            {
                var cmd = new CmdSetStepperActiveCruisePulseWidth(stepperIndex, config.activeModeConfig.cruisingPulseWidth);
                var result = await RunCommand(cmd);
                if (result != "success")
                {
                    return Results.Text($"Failed in set cruising period: '{result}'", "text/html");
                }
            }

            if (config.activeModeConfig.deacceleratingSteps < 1)
            {
                throw new InvalidRequestBodyException($"Invalid count of rampdown periods: {config.activeModeConfig.deacceleratingSteps}");
            }
            else
            {
                ushort[] deacceleratingPeriods = new ushort[config.activeModeConfig.deacceleratingSteps];
                double deacceleratingRate = config.activeModeConfig.endingPulseWidth - config.activeModeConfig.cruisingPulseWidth;
                deacceleratingRate /= config.activeModeConfig.deacceleratingSteps;
                if (deacceleratingRate < 0)
                {
                    throw new InvalidRequestBodyException($"Invalid rampdown periods: '{config.activeModeConfig.endingPulseWidth}', '{config.activeModeConfig.cruisingPulseWidth}'");
                }

                for (int i = 0; i < deacceleratingPeriods.Length; i++)
                {
                    deacceleratingPeriods[i] = (ushort)Math.Round(config.activeModeConfig.cruisingPulseWidth + (i + 1) * deacceleratingRate);
                }

                var totalBatches = (deacceleratingPeriods.Length + PERIODS_PER_BATCH - 1) / PERIODS_PER_BATCH;
                for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    ushort[] batch;

                    if ((batchIndex + 1) * PERIODS_PER_BATCH > deacceleratingPeriods.Length)
                    {
                        batch = new ushort[deacceleratingPeriods.Length - batchIndex * PERIODS_PER_BATCH];
                    }
                    else
                    {
                        batch = new ushort[PERIODS_PER_BATCH];
                    }

                    for (int i = 0; i < batch.Length; i++)
                    {
                        batch[i] = deacceleratingPeriods[batchIndex * PERIODS_PER_BATCH + i];
                    }

                    var cmd = new CmdSetStepperActiveRampdownPulseWidth(stepperIndex, (byte)batchIndex, (byte)totalBatches, batch);
                    var result = await RunCommand(cmd);
                    if (result != "success")
                    {
                        return Results.Text($"Failed in set rampdown periods: '{result}'", "text/html");
                    }
                }
            }

            return Results.Text("success", "text/html");
        }
        else if (command == "setStepperControls")
        {
            byte stepperIndex = jsonRoot.GetProperty("stepperId").GetByte();
            var configs = StaticConfig.Instance.StepperConfigs;

            if (stepperIndex >= configs.Length)
            {
                throw new InvalidRequestBodyException($"Invalid stepper index '{stepperIndex}'");
            }

            var config = configs[stepperIndex];
            var cmd = new CmdSetSteppeControls(stepperIndex,
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
        else if (command == "setStepperEnable")
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
        else if (command == "setStepperForward")
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
        else if (command == "startStepperHomePositioning")
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
        else if (command == "setActiveSteps")
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
        else if (command == "runStepper")
        {
            return await RunStepper(jsonRoot);
        }
        else if (command == "testTimer")
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
        else if (command == "testStepperEnable")
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
        else if (command == "testStepperForward")
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
        else if (command == "testStepperClock")
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
        else if (command == "testStepperForce")
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
        else if (command == "testStepperPulseEnd")
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
        else if (command == "testStepperStateReady")
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
        else if (command == "testStepperActive")
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
    catch (InvalidRequestBodyException e)
    {
        Console.WriteLine($"Error: exception in buildPostCommand(): {e.Message}");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error: exception in MapPost: {e.Message}");
    }

    return TypedResults.NotFound(new { error = "Item not found", command });
});

app.Run();
