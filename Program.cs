using System.Text.Json;
using VirtualOperatorServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BackSocket>();
builder.Services.AddHostedService<BackService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/get/{*command}", (string command) =>
{
    Console.WriteLine($"/get/{command}");
    if(command.Length == 0)
    {
        return Results.Text("Empty command", "text/html");
    }

    return ClientHandlers.GetCommandHandler(command);
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

    return await ClientHandlers.PostCommandHandler(command, jsonRoot, backSocket);
});

app.Run();
