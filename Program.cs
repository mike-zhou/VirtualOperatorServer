using System.Text.Json;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BackSocket>();

var app = builder.Build();

app.UseDefaultFiles(); 
app.UseStaticFiles();

static CommandAndReply buildGetCommand(string command)
{
    CommandAndReply cmd = new([]);

    string[] cmdSegments = command.Split('/');
    if(cmdSegments[0] == "Version")
    {
        cmd = new CmdGetVersion();
    }
    else if(cmdSegments[0] == "GPIOMode")
    {
        cmd = new CmdGetGPIOMode();
    }
    else if(cmdSegments[0] == "GPIO")
    {
        cmd = new CmdReadGPIO();
    }
    else if(cmdSegments[0] == "PeripharalStatus")
    {
        cmd = new CmdPeripheralStatus();
    }
    else
    {
        Console.WriteLine($"Error: unknown GET command: {command}");
    }

    return cmd;
}

static CommandAndReply buildSetBdcPowerOutput(JsonElement jsonRoot)
{
    CmdSetGPIO.GPIO[] gpioArray = [new CmdSetGPIO.GPIO()];
    gpioArray[0].portName = "PC";
    gpioArray[0].bitIndex = 15;
    gpioArray[0].level = jsonRoot.GetProperty("enable").GetBoolean()? (byte)1 : (byte)0;

    return new CmdSetGPIO(gpioArray);
}

static CommandAndReply buildSetBdcControl(JsonElement jsonRoot)
{
    // payload = 
    // { 
    //     action: actionStr,
    //     index: parseInt(indexStr, 10)
    // };

    string? action = jsonRoot.GetProperty("action").GetString();
    int index = jsonRoot.GetProperty("index").GetInt32();

    if(action == null)
        throw new InvalidRequestBodyException($"No valid action in {jsonRoot.GetRawText()}");

    CmdSetGPIO.GPIO[] gpioArray = [new CmdSetGPIO.GPIO(), new CmdSetGPIO.GPIO()];
    switch (index)
    {
        case 0:
            // GP115, GP112
            gpioArray[0].portName = "PI";
            gpioArray[0].bitIndex = 4;
            gpioArray[1].portName = "PB";
            gpioArray[1].bitIndex = 9;
            break;
        case 1:
            // GP2, GP118
            gpioArray[0].portName = "PE";
            gpioArray[0].bitIndex = 4;
            gpioArray[1].portName = "PI";
            gpioArray[1].bitIndex = 7;
            break;
        case 2:
            // GP8, GP5
            gpioArray[0].portName = "PI";
            gpioArray[0].bitIndex = 9;
            gpioArray[1].portName = "PC";
            gpioArray[1].bitIndex = 13;
            break;
        default:
            throw new InvalidRequestBodyException($"Invalid index in {jsonRoot.GetRawText()}");
    }

    if(action == "coast")
    {
        gpioArray[0].level = 0;
        gpioArray[1].level = 0;
    }
    else if(action == "reverse")
    {
        gpioArray[0].level = 0;
        gpioArray[1].level = 1;
    }
    else if(action == "forward")
    {
        gpioArray[0].level = 1;
        gpioArray[1].level = 0;
    }
    else if(action == "brake")
    {
        gpioArray[0].level = 1;
        gpioArray[1].level = 1;
    }
    else
    {
        throw new InvalidRequestBodyException($"Unknown action in {jsonRoot.GetRawText()}");
    }
    
    return new CmdSetGPIO(gpioArray);
}

static CommandAndReply buildPostCommand(string command, JsonElement jsonRoot)
{
    CommandAndReply cmd = new([]);

    string[] cmdSegments = command.Split('/');
    if(cmdSegments[0] == "Echo")
    {
        cmd = new CmdEcho(jsonRoot);
    }
    else if (cmdSegments[0] == "setGpio")
    {
        cmd = new CmdSetGPIO(jsonRoot);
    }
    else if (cmdSegments[0] == "setPowerOutput")
    {
        cmd = new CmdSetPowerOutput(jsonRoot);
    }
    else if (cmdSegments[0] == "setBDCPowerOutput")
    {
        cmd = buildSetBdcPowerOutput(jsonRoot);
    }
    else if (cmdSegments[0] == "setBDCControl")
    {
        cmd = buildSetBdcControl(jsonRoot);
    }
    else
    {
        Console.WriteLine($"Error: unknown POST command: {command}");
    }

    return cmd;
}

app.MapGet("/get/{*command}", async(string command, BackSocket backSocket) =>
{
    Console.WriteLine($"/get/{command}");
    if(command.Length == 0)
    {
        return Results.Text("Empty command", "text/html");
    }

    CommandAndReply cmd = buildGetCommand(command);
    if(cmd.Command.Length > 0)
    {
        cmd.Reply = await backSocket.SendAndReceive(cmd.Command);
        return Results.Text(cmd.ParseReply(), "text/html");
    }

    return Results.Text("", "text/html");
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
        CommandAndReply cmd = buildPostCommand(command, jsonRoot);
        if(cmd.Command.Length > 0)
        {
            cmd.Reply = await backSocket.SendAndReceive(cmd.Command);
            return Results.Text(cmd.ParseReply(), "text/html");
        }
    }
    catch(InvalidRequestBodyException e)
    {
        Console.WriteLine($"Error: exception in buildPostCommand(): {e.Message}");
    }
    catch(Exception e)
    {
        Console.WriteLine($"Error: exception in MapPost: {e.Message}");
    }

    return Results.Text("", "text/html");
});

app.Run();
