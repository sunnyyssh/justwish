using Ardalis.Result;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Justwish.Users.Application;

public sealed class SendEmailVerificationHandler : ICommandHandler<SendEmailVerificationCommand, Result>
{
    private readonly IRequestClient<SendEmailVerificationRequest> _requestClient;
    private readonly IEmailVerificationIssuer _emailVerificationIssuer;
    private readonly ILogger<SendEmailVerificationHandler> _logger;

    public SendEmailVerificationHandler(IRequestClient<SendEmailVerificationRequest> requestClient,
        IEmailVerificationIssuer emailVerificationIssuer, ILogger<SendEmailVerificationHandler> logger)
    {
        _requestClient = requestClient;
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
        
        var sendEmailRequest = new SendEmailVerificationRequest(request.Email, code);
        var sendEmailResponse =
            await _requestClient.GetResponse<SendEmailVerificationResponse>(sendEmailRequest, cancellationToken);

        return sendEmailResponse.Message.Success
            ? Result.Success()
            : Result.Error("Failed to send email verification code");
    }
}