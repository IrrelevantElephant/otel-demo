using MassTransit;
using Messages;
using Shared;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();

builder.Logging.AddJsonConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ConfigureOpenTelemetry("api", "1.0");

builder.Services.ConfigureMassTransit(appSettings!.MassTransitConfig);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var logger = app.Services.GetService<ILogger<Program>>();

var config = JsonSerializer.Serialize(appSettings);

logger!.LogInformation("This is the config: {config}", config);

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/publish", async (IBus bus, CancellationToken cancellationToken) =>
{
    var message = new HelloMessage("world");
    await bus.Publish(message, cancellationToken);
    return Results.Created();
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
