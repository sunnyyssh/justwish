using FastEndpoints;
using FluentValidation;
using Justwish.Users.Application;
using Justwish.Users.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public sealed class VerifyEmailCodeEndpoint 
    : Endpoint<
        VerifyEmailCodeEndpoint.EmailCodeRequest, 
        Ok<VerifyEmailCodeEndpoint.VerificationStatusResponse>>
{
    private readonly ISender _sender;

    public VerifyEmailCodeEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("registration/verify-email-code");
        Validator<EmailCodeRequestValidator>();
        AllowAnonymous();
    }

    public override async Task<Ok<VerificationStatusResponse>> ExecuteAsync(EmailCodeRequest req, CancellationToken ct)
    {
        bool verified = await _sender.Send(new VerifyEmailCommand(req.Email, req.Code), ct);
        return TypedResults.Ok(new VerificationStatusResponse(verified));
    }

    public record EmailCodeRequest(string Email, int Code);
    
    public record VerificationStatusResponse(bool Verified);

    public sealed class EmailCodeRequestValidator : Validator<EmailCodeRequest>
    {
        public EmailCodeRequestValidator(IUserBusinessRulePredicates rulePredicates)
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .MustAsync(async (email, _) => await rulePredicates.IsUserEmailFree(email))
                .WithMessage("Email is already in use");
        }
    }
}