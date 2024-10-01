using Justwish.Users.Domain;
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

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IProfilePhotoRepository, EfProfilePhotoRepository>();
        services.AddScoped<IUserReadRepository>(sp => sp.GetRequiredService<IUserRepository>());
        services.AddScoped<IProfilePhotoReadRepository>(sp => sp.GetRequiredService<IProfilePhotoRepository>());

        return services;
    }
}