using Handler;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddJsonConsole();

var appSettings = builder.Configuration.Get<AppSettings>();

builder.Services.ConfigureOpenTelemetry("handler", "1.0");

builder.Services.ConfigureMassTransit(appSettings!.MassTransitConfig, (busRegistrationConfigurator) =>
{
    busRegistrationConfigurator.RegisterConsumer<HelloMessageConsumer>();
});

var host = builder.Build();

host.Run();
