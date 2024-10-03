using FluentEmail.Core;
using Justwish.Notifications.Contracts;
using MassTransit;

namespace Justwish.Notifications.Application;

public class SendEmailVerificationConsumer : IConsumer<SendEmailVerificationRequest>
{
    private readonly IFluentEmailFactory _emailFactory;

    public SendEmailVerificationConsumer(IFluentEmailFactory emailFactory)
    {
        _emailFactory = emailFactory;
    }

    public async Task Consume(ConsumeContext<SendEmailVerificationRequest> context)
    {
        var message = _emailFactory.Create()
            .To(context.Message.Email)
            .Subject("Verify your email address")
            .Body($"Please verify your email address. Don't know how. Ah, here is the code: {context.Message.Code}")
            .SendAsync();

        await context.RespondAsync(new SendEmailVerificationResponse(true));
    }
}
