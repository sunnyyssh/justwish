using System.Text.Json;
using FastEndpoints;
using Justwish.Users.Application;
using Justwish.Users.Infrastructure;
using Justwish.Users.WebApi;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.WebHost.ConfigureKestrel(opts => 
{
    opts.Limits.MaxRequestBodySize = 5 * 1024 * 1024;
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.ValidJwtTokenTypePolicy, policy =>
    {
        policy.RequireClaim(JwtTokenConstants.TokenTypeClaimName, JwtTokenConstants.AccessTokenType);
    });

builder.Services.Configure<JsonOptions>(opts => 
{
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.ConfigureOptions<JwtBearerConfigureOptions>();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
});

builder.Services.AddFastEndpoints(options =>
{
    if (!builder.Environment.IsDevelopment())
    {
        options.Filter = endpointType => endpointType != typeof(HelloWorldEndpoint);
    }
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapFastEndpoints(config => 
{
    config.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

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
