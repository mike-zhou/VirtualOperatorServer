using VirtualOperatorServer.CardMappings;
using VirtualOperatorServer.Coordinates;
using VirtualOperatorServer.Services;
using VirtualOperatorServer.Operation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BackSocket>();
builder.Services.AddSingleton<LowLevelAccessHandlers>();
builder.Services.AddSingleton<CoordinateStore>();
builder.Services.AddSingleton<CardMappingStore>();
builder.Services.AddSingleton<Operations>();
builder.Services.AddHostedService<BackService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapCoordinateApi();
app.MapCardMappingApi();
app.MapOperationsApi();
app.MapLowLevelAccessApi();

app.Run();
