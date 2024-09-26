using FastEndpoints;
using FluentValidation;
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
        public RegistrationRequestValidator()
        {
            RuleFor(x => x.Username)
                .Username()
                .MustAsync(async (username, _) =>
                {
                    var rulePredicates = Resolve<IUserBusinessRulePredicates>();
                    return await rulePredicates.IsUsernameFree(username);
                })
                .WithMessage("Username is not free");
            
            RuleFor(x => x.Email)
                .EmailAddress()
                .MustAsync(async (email, _) =>
                {
                    var rulePredicates = Resolve<IUserBusinessRulePredicates>();
                    return await rulePredicates.IsUserEmailFree(email);
                })
                .WithMessage("Email is not free");

            RuleFor(x => x.Password)
                .Password();
        }
    }
}