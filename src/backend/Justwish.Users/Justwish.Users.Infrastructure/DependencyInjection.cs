﻿using Justwish.Users.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        
        services.AddDbContext<ApplicationDbContext>(opts =>
        {
            if (environment.IsDevelopment())
            {
                opts.EnableSensitiveDataLogging();
                opts.UseAsyncSeeding(SeedDevelopmentData.SeedAsync);
            }
            opts.UseNpgsql(configuration.GetConnectionString("ApplicationConnection"));
        });

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IUserReadRepository>(sp => sp.GetRequiredService<IUserRepository>()); 
        
        return services;
    }
}