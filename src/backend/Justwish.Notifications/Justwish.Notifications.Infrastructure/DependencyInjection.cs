using Justwish.Notifications.Application;
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
            config.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter(includeNamespace: false));

            config.UsingRabbitMq(static (ctx, conf) => 
            {
                var configuration = ctx.GetRequiredService<IConfiguration>();
                string host = configuration.GetRequiredSection("RabbitMqOptions:Host").Value!;
                string username = configuration.GetRequiredSection("RabbitMqOptions:User").Value!;
                string password = configuration.GetRequiredSection("RabbitMqOptions:Password").Value!;

                conf.Host(host, "/", h => 
                {
                    h.Username(username);
                    h.Password(password);
                });

                conf.Durable = true;
                
                conf.ConfigureEndpoints(ctx, new SnakeCaseEndpointNameFormatter(includeNamespace: false));
            });
        });

        return services;
    }
}
