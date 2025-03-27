using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BackSocket>();

var app = builder.Build();

app.UseDefaultFiles(); 
app.UseStaticFiles();

static CommandAndReply onCommand(string command)
{
    Console.WriteLine($"onCommand: {command}");
    
    CommandAndReply cmd = new(Array.Empty<byte>());

    string[] cmdSegments = command.Split('/');
    if(cmdSegments[0] == "GetVersion")
    {
        cmd = new CmdGetVersion();
    }
    else if(cmdSegments[0] == "GetVersion")
    {
        cmd = new CmdGetGPIO();
    }
    else
    {
        Console.WriteLine($"Error: unknown command: {command}");
    }

    return cmd;
}

app.MapGet("/api/{*command}", (string command, BackSocket backSocket) =>
{
    if(command.Length == 0)
    {
        return Results.Text("Empty command", "text/html");
    }

    CommandAndReply cmd = onCommand(command);
    cmd.Reply = backSocket.SendCommand(cmd.Command);
    return Results.Text(cmd.ParseReply(), "text/html");
});

app.Run();
