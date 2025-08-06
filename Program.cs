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

app.MapPost("/post/{*command}", async(HttpRequest request, string command, BackSocket backSocket) =>
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
        else if (command == "saveStepperTimer")
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
