using Shared;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();

builder.Logging.AddJsonConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ConfigureOpenTelemetry("emailapi", "1.0");

builder.Services.ConfigureMassTransit(appSettings!.MassTransitConfig);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/email", () =>
{
    app.Logger.LogInformation("Sending an email!");

    return Results.Created();
});

app.Run();
