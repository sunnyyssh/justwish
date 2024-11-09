using Justwish.Users.Domain;
using Microsoft.Extensions.Logging;

namespace Justwish.Users.Application;

public sealed class VerifyEmailHandler : ICommandHandler<VerifyEmailCommand, bool>
{
    private readonly IEmailVerifier _verifier;
    private readonly ILogger<VerifyEmailHandler> _logger;

    public VerifyEmailHandler(IEmailVerifier verifier, ILogger<VerifyEmailHandler> logger)
    {
        _verifier = verifier;
        _logger = logger;
    }
    
    public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        bool verified = await _verifier.VerifyEmailAsync(request.Email, request.Code);
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (verified)
        {
            _logger.LogInformation("Email verified");
        }
        else
        {
            _logger.LogInformation("Email not verified");
        }
        return verified;
    }
}