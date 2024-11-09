using FluentEmail.Core;
using Justwish.Notifications.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Justwish.Notifications.Application;

public class SendEmailVerificationConsumer : IConsumer<SendEmailVerificationRequest>
{
    private readonly IFluentEmailFactory _emailFactory;
    private readonly ILogger<SendEmailVerificationConsumer> _logger;

    public SendEmailVerificationConsumer(IFluentEmailFactory emailFactory, ILogger<SendEmailVerificationConsumer> logger)
    {
        _emailFactory = emailFactory;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendEmailVerificationRequest> context)
    {
        await _emailFactory.Create()
            .To(context.Message.Email)
            .Subject("Verify your email address")
            .Body($"Please verify your email address. Don't know how. Ah, here is the code: {context.Message.Code}")
            .SendAsync();

        _logger.LogInformation("Verification email sent to {Email}", context.Message.Email);
    }
}
