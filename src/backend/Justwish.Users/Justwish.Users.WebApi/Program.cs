using System.Text.Json;
using FastEndpoints;
using Justwish.Users.Application;
using Justwish.Users.Infrastructure;
using Justwish.Users.WebApi;
using Justwish.Users.WebApi.ApiKeyAuth;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddSingleton<IApiKeyValidator, ConfigurationApiKeyValidator>();

// builder.Services.AddAuthentication("Default");
builder.Services.AddAuthentication(ApiKeyConstants.AuthenticationScheme)
    .AddScheme<ApiKeySchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyConstants.AuthenticationScheme, null);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ApiKeyConstants.PolicyName, policy =>
    {
        policy.RequireClaim(ApiKeyConstants.ClaimType);
        policy.AddAuthenticationSchemes(ApiKeyConstants.AuthenticationScheme);
    });

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
});

builder.Services.AddMassTransit(config =>
{
    config.UsingInMemory();
});

builder.Services.AddFastEndpoints(options =>
{
    if (!builder.Environment.IsDevelopment())
    {
        options.Filter = endpointType => endpointType != typeof(HelloWorldEndpoint);
    }
});

builder.Services.Configure<JsonOptions>(opts =>
{
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapFastEndpoints();

if (app.Environment.IsDevelopment())
{
    MigrateDatabase(app);
}

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

public partial class Program; 
