using FastEndpoints;
using FluentValidation;
using FluentValidation.Validators;
using Justwish.Users.Application;
using Justwish.Users.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public sealed class CreateUserEndpoint 
    : Endpoint<
        CreateUserEndpoint.RegistrationRequest, 
        Results<
            Ok<CreateUserEndpoint.RegisteredResponse>, 
            BadRequest<string>>>
{
    private readonly ISender _sender;

    public CreateUserEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("registration/create");
        Validator<RegistrationRequestValidator>();
        AllowAnonymous();
    }

    public override async Task<Results<Ok<RegisteredResponse>, BadRequest<string>>> 
        ExecuteAsync(RegistrationRequest req, CancellationToken ct)
    {
        var result = await _sender.Send(new CreateUserCommand(req.Username, req.Email, req.Password), ct);
        if (result.IsEmailNotVerified)
        {
            return TypedResults.BadRequest("Email is not verified");
        }
        return TypedResults.Ok(new RegisteredResponse(result.User.Id));
    }

    public record RegistrationRequest(string Username, string Email, string Password);
    
    public record RegisteredResponse(Guid? UserId);
    
    public class RegistrationRequestValidator : Validator<RegistrationRequest>
    {
        public RegistrationRequestValidator(IUserBusinessRulePredicates rulePredicates)
        {
            RuleFor(x => x.Username)
                .MinimumLength(3)
                .MaximumLength(32)
                .Matches(@"^[a-z0-9_]+$")
                .WithMessage("Username must contain only lowercase alphanumeric characters and underscores")
                .MustAsync(async (username, _) => await rulePredicates.IsUsernameFree(username))
                .WithMessage("Username is not free");
            
            RuleFor(x => x.Email)
                .EmailAddress()
                .MustAsync(async (email, _) => await rulePredicates.IsUserEmailFree(email))
                .WithMessage("Email is not free");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .MaximumLength(32)
                .Matches(@"^[a-zA-Z0-9_]+$")
                .WithMessage("Password must contain only alphanumeric characters and underscores");
        }
    }
}