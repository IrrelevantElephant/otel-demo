using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, MassTransitConfig massTransitConfig, Action<IBusRegistrationConfigurator> additionalConfiguration)
    {
        services.AddMassTransit(busRegistrationConfigurator =>
        {
            busRegistrationConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(massTransitConfig.Uri, "/", h =>
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
        ConfigureMassTransit(services, massTransitConfig, (bus) => { });
        return services;
    }
}
