using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Justwish.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, 
        IHostEnvironment environment)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumers(typeof(Application.DependencyInjection).Assembly);

            config.UsingInMemory((ctx, inMemoryConfigure) =>
            {
                inMemoryConfigure.UseDelayedMessageScheduler();
                inMemoryConfigure.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
