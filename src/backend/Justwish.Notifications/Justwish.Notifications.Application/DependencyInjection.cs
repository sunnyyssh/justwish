using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Justwish.Notifications.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration, 
        IHostEnvironment environment)
    {
        SmtpEmailSenderOptions opts = new()
        {
            Host = configuration.GetRequiredSection("SmtpEmailSenderOptions:Host").Value!,
            Port = int.Parse(configuration.GetRequiredSection("SmtpEmailSenderOptions:Port").Value!),
            NoreplyEmail = configuration.GetRequiredSection("SmtpEmailSenderOptions:NoreplyEmail").Value!,
            FromName = configuration.GetRequiredSection("SmtpEmailSenderOptions:FromName").Value!,
        };
        if (environment.IsProduction())
        {
            opts.Password = configuration.GetRequiredSection("SmtpEmailSenderOptions:Password").Value!;
        }

        var fluentEmailBuilder = services.AddFluentEmail(opts.NoreplyEmail, opts.FromName);
        if (environment.IsDevelopment())
        {
            fluentEmailBuilder.AddSmtpSender(opts.Host, opts.Port);
        }

        return services;
    }
}
