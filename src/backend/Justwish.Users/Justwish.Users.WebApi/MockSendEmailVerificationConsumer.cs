using Justwish.Users.Contracts;
using MassTransit;

namespace Justwish.Users.WebApi;

// ATTENTION: It should be used only for development as mock for remote consumer.
public sealed class MockSendEmailVerificationConsumer : IConsumer<SendEmailVerificationRequest>
{
    private readonly ILogger<MockSendEmailVerificationConsumer> _logger;

    public MockSendEmailVerificationConsumer(ILogger<MockSendEmailVerificationConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<SendEmailVerificationRequest> context)
    {
        await context.RespondAsync(new SendEmailVerificationResponse(true));
        _logger.LogInformation("MockSendEmailVerificationConsumer received email verification request: {Request}", context.Message);   
    }
}