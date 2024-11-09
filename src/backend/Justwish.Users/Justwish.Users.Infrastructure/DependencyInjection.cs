using Justwish.Users.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Justwish.Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = configuration.GetConnectionString("RedisConnection");
        });

        if (!environment.IsEnvironment("Test"))
        {
            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                if (environment.IsDevelopment())
                {
                    opts.EnableSensitiveDataLogging();
                }

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("ApplicationConnection"));
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                opts.UseNpgsql(dataSource);
            });
        }

        services.AddMassTransit(config =>
        {
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

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IProfilePhotoRepository, EfProfilePhotoRepository>();
        services.AddScoped<IUserReadRepository>(sp => sp.GetRequiredService<IUserRepository>());
        services.AddScoped<IProfilePhotoReadRepository>(sp => sp.GetRequiredService<IProfilePhotoRepository>());

        return services;
    }
}