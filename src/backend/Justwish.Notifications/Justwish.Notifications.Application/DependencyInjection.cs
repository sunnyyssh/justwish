using System.Net;
using System.Net.Mail;
using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Justwish.Notifications.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        SmtpEmailSenderOptions opts = new()
        {
            Host = configuration.GetRequiredSection("SmtpEmailSenderOptions:Host").Value!,
            Port = int.Parse(configuration.GetRequiredSection("SmtpEmailSenderOptions:Port").Value!),
            NoreplyEmail = configuration.GetRequiredSection("SmtpEmailSenderOptions:NoreplyEmail").Value!,
            FromName = configuration.GetRequiredSection("SmtpEmailSenderOptions:FromName").Value!,
            Password = configuration.GetRequiredSection("SmtpEmailSenderOptions:Password").Value!
        };

        services.AddFluentEmail(opts.NoreplyEmail, opts.FromName)
            .AddSmtpSender(opts.Host, opts.Port, opts.NoreplyEmail, opts.Password);

        return services;
    }
}
