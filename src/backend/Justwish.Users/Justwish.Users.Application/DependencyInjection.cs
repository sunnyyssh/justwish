using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Justwish.Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, DefaultPasswordHasher>();
        
        services.AddScoped<IVerificationCodeGenerator, DefaultVerificationCodeGenerator>();
        services.AddScoped<IEmailVerificationService, CacheEmailVerificationService>();
        services.AddScoped<IEmailVerificationIssuer>(sp => sp.GetRequiredService<IEmailVerificationService>());
        services.AddScoped<IEmailVerificationChecker>(sp => sp.GetRequiredService<IEmailVerificationService>());
        services.AddScoped<IEmailVerifier>(sp => sp.GetRequiredService<IEmailVerificationService>());

        services.Configure<EmailVerificationOptions>(opts =>
        {
            configuration.GetRequiredSection("EmailVerificationOptions").Bind(opts);
        });
        
        services.AddScoped<IUserBusinessRulePredicates, DefaultUserBusinessRulePredicates>();

        services.AddScoped<IJwtEncoder, JwtEncoder>();
        services.AddScoped<IJwtRefreshTokenStorage, CacheRefreshTokenStorage>();
        services.AddScoped<IJwtService, JwtService>();

        services.Configure<JwtOptions>(opts =>
        {
            configuration.GetSection("JwtOptions").Bind(opts);
        });

        return services;
    }
}