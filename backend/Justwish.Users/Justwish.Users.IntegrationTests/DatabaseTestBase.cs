using System;
using Justwish.Users.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Justwish.Users.IntegrationTests;

public abstract class DatabaseTestBase : IAsyncDisposable
{
    private readonly PostgreSqlContainer _container;

    public ApplicationDbContext Context { get; private set; }

    protected DatabaseTestBase()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("Users")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
        _container.StartAsync().GetAwaiter().GetResult();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_container.GetConnectionString());
        dataSourceBuilder.EnableDynamicJson();

        options.UseNpgsql(dataSourceBuilder.Build());
        options.EnableSensitiveDataLogging();
        options.ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));

        Context = new ApplicationDbContext(options.Options);
        if (Context.Database.GetPendingMigrations().Any())
        {   
            Context.Database.Migrate();
        }
        TestData.SeedData(Context);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
        await Context.DisposeAsync();
    }
}
