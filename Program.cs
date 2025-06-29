using System.Diagnostics;
using System.Text.Json;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Services;

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
