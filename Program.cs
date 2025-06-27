using System.Text.Json;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BackSocket>();
builder.Services.AddHostedService<BackService>();

var app = builder.Build();

app.UseDefaultFiles(); 
app.UseStaticFiles();

app.MapGet("/get/{*command}", async(string command, BackSocket backSocket) =>
{
    Console.WriteLine($"/get/{command}");
    if(command.Length == 0)
    {
        return Results.Text("Empty command", "text/html");
    }

    CommandAndReply cmd = CommandFactory.BuildGetCommand(command);
    if(cmd.Command.Length > 0)
    {
        cmd.Reply = await backSocket.SendAndReceiveAsync(cmd.Command);
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
        CommandAndReply cmd = CommandFactory.BuildPostCommand(command, jsonRoot);
        if(cmd.Command.Length > 0)
        {
            cmd.Reply = await backSocket.SendAndReceiveAsync(cmd.Command);
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
