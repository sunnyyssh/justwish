using Justwish.Users.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Justwish.Users.FunctionalTests;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("Users")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = builder.Build();
        host.Start();

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
        dbContext.Database.EnsureCreated();
        TestData.PopulateTestData(dbContext);

        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _postgresContainer.StartAsync().GetAwaiter().GetResult();

        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration(c =>
        {
            c.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false);
        });

        builder.ConfigureServices((context, services) =>
        {

            var dbContextDescriptor =
            services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
                var builderDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptionsBuilder));
            }

            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts.EnableSensitiveDataLogging();
                opts.ConfigureWarnings(warnings =>
                {
                    // warnings.Log(RelationalEventId.PendingModelChangesWarning);
                });

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(_postgresContainer.GetConnectionString());
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                opts.UseNpgsql(_postgresContainer.GetConnectionString());
            });

            var cacheDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDistributedCache));
            if (cacheDescriptor is not null)
            {
                services.Remove(cacheDescriptor);
            }

            services.AddDistributedMemoryCache();

            services.AddMassTransitTestHarness(configure =>
            {
                configure.AddConsumer<SendEmailVerificationConsumer>();
                configure.UsingInMemory((ctx, inMemoryConfigure) =>
                {
                    inMemoryConfigure.UseDelayedMessageScheduler();
                    inMemoryConfigure.ConfigureEndpoints(ctx);
                });
            });
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}