using Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddJsonConsole();
builder.Services.AddHttpClient();

var appSettings = builder.Configuration.Get<AppSettings>();

builder.ConfigureOpenTelemetry("handler", "1.0");

builder.Services.ConfigureMassTransit(appSettings!.MassTransitConfig, (busRegistrationConfigurator) =>
{
    busRegistrationConfigurator.AddConsumer<HelloMessageConsumer>();
});

var host = builder.Build();

host.Run();
