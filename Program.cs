using System.Diagnostics;
using System.Text.Json;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Facade;
using VirtualOperatorServer.Services;
using VirtualOperatorServer.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BackSocket>();
builder.Services.AddHostedService<BackService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

string ProcessGetCommand(string command)
{
    CommandAndReply cmd = new([]);

    string[] cmdSegments = command.Split('/');
    if (cmdSegments[0] == "Version")
    {
        return ViewFactory.CreateVersion();
    }
    else if (cmdSegments[0] == "GPIO")
    {
        return ViewFactory.CreateGpioTable();
    }
    else if (cmdSegments[0] == "PeripharalStatus")
    {
        return ViewFactory.CreatePeripheralStatus();
    }
    else if (cmdSegments[0] == "Encoders")
    {
        return ViewFactory.CreateEncoders();
    }
    else if (cmdSegments[0] == "DynamicStatus")
    {
        return ViewFactory.CreateDynamicStatus();
    }
    else if (cmdSegments[0] == "HtmlSteppers")
    {
        return ViewFactory.CreateSteppers();
    }
    else if (cmdSegments[0] == "Status")
    {
        return StatusFacade.CreateStatusJsonString();
    }
    else
    {
        Console.WriteLine($"Error: unknown GET command: {command}");
    }

    return "";
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
            if (cmd.Command.Length > 0)
            {
                cmd.Reply = await backSocket.SendAndReceiveAsync(cmd.Command);

                bool success;
                string reason;

                (success, reason) = cmd.ParseReply();
                if (success)
                {
                    return Results.Text("success", "text/html");
                }
                else
                {
                    return Results.Text($"failure: {reason}", "text/html");
                }
            }
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
                default:
                    throw new Exception($"Invalid classification '{classification}' in POST command '{command}'");
            }

            StaticConfig.Instance.SaveStepperConfigs();
            return Results.Text("success", "text/html");
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
