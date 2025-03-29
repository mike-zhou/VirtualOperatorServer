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
    CommandAndReply cmd = new(Array.Empty<byte>());

    string[] cmdSegments = command.Split('/');
    if(cmdSegments[0] == "Version")
    {
        cmd = new CmdGetVersion();
    }
    else if(cmdSegments[0] == "GPIO")
    {
        cmd = new CmdGetGPIO();
    }
    else
    {
        Console.WriteLine($"Error: unknown GET command: {command}");
    }

    return cmd;
}

static CommandAndReply buildPostCommand(string command, JsonElement jsonRoot)
{
    CommandAndReply cmd = new([]);

    string[] cmdSegments = command.Split('/');
    try
    {
        if(cmdSegments[0] == "Echo")
        {
            cmd = new CmdEcho(jsonRoot);
        }
        else
        {
            Console.WriteLine($"Error: unknown POST command: {command}");
        }
    }
    catch(InvalidRequestBodyException e)
    {
        Console.WriteLine($"Error: exception in buildPostCommand(): {e.Message}");
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
    cmd.Reply = await backSocket.SendAndReceive(cmd.Command);
    return Results.Text(cmd.ParseReply(), "text/html");
});

app.MapPost("/post/{*command}", async(HttpRequest request, string command, BackSocket backSocket) =>
{
    Console.WriteLine($"/post/{command}");

    using var jsonDoc = await JsonDocument.ParseAsync(request.Body);
    var jsonRoot = jsonDoc.RootElement;
    CommandAndReply cmd = buildPostCommand(command, jsonRoot);
    cmd.Reply = await backSocket.SendAndReceive(cmd.Command);

    return Results.Text(cmd.ParseReply(), "text/html");
});

app.Run();
