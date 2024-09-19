using System.Text.Json;
using FastEndpoints;
using Justwish.Users.Application;
using Justwish.Users.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
});

builder.Services.AddMassTransit(config =>
{
    config.UsingInMemory();
});

builder.Services.AddFastEndpoints();

builder.Services.Configure<JsonOptions>(opts =>
{
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

app.MapFastEndpoints();

MigrateDatabase(app);

app.Run();
return;


static void MigrateDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}