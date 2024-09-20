using Justwish.Users.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Justwish.Users.FunctionalTests;

public sealed class SendEmailVerificationConsumer : IConsumer<SendEmailVerificationRequest>
{
    private readonly ILogger<SendEmailVerificationConsumer> _logger;

    public SendEmailVerificationConsumer(ILogger<SendEmailVerificationConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<SendEmailVerificationRequest> context)
    {
        await context.RespondAsync(new SendEmailVerificationResponse(true));
        
        _logger.LogInformation("SendEmailVerificationRequest consumed for {Email}", context.Message.Email);
    }
}