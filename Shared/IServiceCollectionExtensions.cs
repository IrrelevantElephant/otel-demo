using MassTransit;
using MassTransit.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.ResourceDetectors.Container;
using OpenTelemetry.ResourceDetectors.Host;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, MassTransitConfig massTransitConfig, Action<IBusRegistrationConfigurator> additionalConfiguration)
    {
        services.AddMassTransit(busRegistrationConfigurator =>
        {
            busRegistrationConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(massTransitConfig.Uri, massTransitConfig.VirtualHost, h =>
                {
                    h.Username(massTransitConfig.Username);
                    h.Password(massTransitConfig.Password);
                });

                cfg.ConfigureEndpoints(context);
            });

            additionalConfiguration(busRegistrationConfigurator);
        });

        return services;
    }

    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, MassTransitConfig massTransitConfig)
    {
        ConfigureMassTransit(services, massTransitConfig, (additionalConfiguration) => { });
        return services;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder hostApplicationBuilder, string serviceName, string serviceVersion, Action<TracerProviderBuilder> additionalTraceConfiguration)
    {
        Action<ResourceBuilder> appResourceBuilder =
            resource => resource
                .AddService(serviceName, serviceVersion: serviceVersion)
                .AddDetector(new ContainerResourceDetector())
                .AddDetector(new HostDetector());

        hostApplicationBuilder.Services.AddOpenTelemetry()
            .ConfigureResource(appResourceBuilder)
            .WithTracing(tracerBuilder =>
            {
                additionalTraceConfiguration(tracerBuilder);
                tracerBuilder.AddSource(DiagnosticHeaders.DefaultListenerName);
                tracerBuilder.AddOtlpExporter();
            });

        hostApplicationBuilder.Logging.AddOpenTelemetry(options => options.AddOtlpExporter());

        return hostApplicationBuilder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder hostApplicationBuilder, string serviceName, string serviceVersion)
    {
        ConfigureOpenTelemetry(hostApplicationBuilder, serviceName, serviceVersion, (additionalTraceConfiguration) => { });
        return hostApplicationBuilder;
    }
}
