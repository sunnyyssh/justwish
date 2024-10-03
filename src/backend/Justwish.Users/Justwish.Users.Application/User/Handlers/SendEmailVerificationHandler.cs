using Ardalis.Result;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Justwish.Users.Application;

public sealed class SendEmailVerificationHandler : ICommandHandler<SendEmailVerificationCommand, Result>
{
    private readonly IEmailVerificationIssuer _emailVerificationIssuer;
    private readonly ILogger<SendEmailVerificationHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public SendEmailVerificationHandler(IPublishEndpoint publishEndpoint,
        IEmailVerificationIssuer emailVerificationIssuer, ILogger<SendEmailVerificationHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _emailVerificationIssuer = emailVerificationIssuer;
        _logger = logger;
    }
    
    public async Task<Result> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var code = await _emailVerificationIssuer.IssueCodeAsync(request.Email);
        if (!code.IsSuccess)
        {
            return Result.Error();
        }
        
        _logger.LogInformation("Issued new verification code for {Email}", request.Email);

        var sendEmailRequest = new SendEmailVerificationRequest(request.Email, code.Value);

        await _publishEndpoint.Publish(sendEmailRequest);
        _logger.LogInformation("SendEmailVerificationRequest published");
        
        return Result.Success();
    }
}