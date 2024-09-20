using Justwish.Users.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Justwish.Users.FunctionalTests;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
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
        SeedTestData.PopulateTestData(dbContext);

        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor is not null)
            {
                services.Remove(services.Single(d => d.ServiceType == typeof(ApplicationDbContext)));
                services.Remove(dbContextDescriptor);
            }


            string databaseName = Guid.NewGuid().ToString(); // Every test case should use its own database.
            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts.EnableSensitiveDataLogging();
                opts.ConfigureWarnings(warnings =>
                {
                    warnings.Log(RelationalEventId.PendingModelChangesWarning);
                });
                
                opts.UseSqlite($"Data Source={databaseName}.db");
            }, ServiceLifetime.Transient);
            
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
}