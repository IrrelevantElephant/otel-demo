using Database;
using Domain;
using MassTransit;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();

builder.Logging.AddJsonConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ConfigureOpenTelemetry("api", "1.0");

builder.Services.ConfigureMassTransit(appSettings!.MassTransitConfig);

builder.Services.ConfigureRedis(appSettings);
builder.Services.ConfigureDatabase(appSettings);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/greetings", async ([FromServices]IBus bus, [FromServices]IConnectionMultiplexer muxer, CancellationToken cancellationToken, [FromBody]CreateGreetingRequest request) =>
{
    var greeting = new Greeting(Guid.NewGuid(), request.Subject, request.Message);

    var redis = muxer.GetDatabase();

    var json = JsonSerializer.Serialize(greeting);

    await redis.StringSetAsync(greeting.Id.ToString(), json);

    var message = new GreetingCreated(greeting.Id);

    await bus.Publish(message, cancellationToken);
    return Results.Created();
})
.WithName("CreateGreeting")
.WithOpenApi();

app.MapPost("/greetings/migrate", async ([FromServices] GreetingContext context) =>
{
    await context.Database.MigrateAsync();
    return Results.Ok();
});

app.MapGet("/greetings/{id}", async ([FromServices]GreetingContext context, Guid id) =>
{
    var greeting = await context.Greetings.FirstOrDefaultAsync(x => x.Id == id);

    if (greeting == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(greeting);
});

app.Run();

record CreateGreetingRequest(string Subject, string Message);