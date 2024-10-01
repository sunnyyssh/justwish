using FastEndpoints;
using FluentValidation;
using Justwish.Users.Application;
using Justwish.Users.Domain;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public sealed class SendEmailCodeEndpoint : Endpoint<SendEmailCodeEndpoint.EmailRequest, Ok>
{
    private readonly ISender _sender;

    public SendEmailCodeEndpoint(ISender sender)
    {
        _sender = sender;
    }
    
    public override void Configure() 
    {
        Post("/registration/send-email-code");
        Validator<EmailRequestValidator>();
        AllowAnonymous();
    }


    public override async Task<Ok> ExecuteAsync(EmailRequest req, CancellationToken ct)
    {
        await _sender.Send(new SendEmailVerificationCommand(req.Email), ct);
        return TypedResults.Ok();
    }

    public record EmailRequest(string Email);

    public sealed class EmailRequestValidator : Validator<EmailRequest>
    {
        public EmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .MustAsync(async (email, _) =>
                {
                    var rulePredicates = Resolve<IUserBusinessRulePredicates>();
                    return await rulePredicates.IsUserEmailFree(email);
                })
                .WithMessage("Email is already in use");
        }
    }
}
