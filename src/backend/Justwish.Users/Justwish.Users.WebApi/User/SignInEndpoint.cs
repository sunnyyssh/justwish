using Ardalis.Result;
using FastEndpoints;
using FluentValidation;
using Justwish.Users.Application;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using Justwish.Users.WebApi.ApiKeyAuth;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
// ReSharper disable ConvertTypeCheckPatternToNullCheck

namespace Justwish.Users.WebApi;

public sealed class SignInEndpoint : Endpoint<SignInEndpoint.SignInRequest, Results<Ok<SignInEndpoint.SignInResponse>, BadRequest<string>>>
{
    private readonly ISender _sender;

    public SignInEndpoint(ISender sender)
    {
        _sender = sender;
    }
    
    public override void Configure()
    {
        Post("auth/signin");
        AllowAnonymous();
        Policies(ApiKeyConstants.PolicyName);
    }

    public override async Task<Results<Ok<SignInResponse>, BadRequest<string>>> ExecuteAsync(SignInRequest req, CancellationToken ct)
    {
        var result = req switch
        {
            (string email, _, string password) => await _sender.Send(new SignInWithEmailCommand(email, password), ct),
            (_, string username, string password) => await _sender.Send(new SignInWithUsernameCommand(username, password), ct),
            _ => Result.Error("Username or email must be set")
        };

        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(string.Join("; ", result.Errors));
        }

        var response = new SignInResponse(result.Value.AccessToken.Token, result.Value.RefreshToken.Token);
        return TypedResults.Ok(response);
    }

    public record SignInRequest(string? Email, string? Username, string Password);
    
    public record SignInResponse(string AccessToken, string RefreshToken);
    
    public sealed class SignInValidator : Validator<SignInRequest>
    {
        public SignInValidator(IUserBusinessRulePredicates predicates)
        {
            When(x => string.IsNullOrWhiteSpace(x.Username), () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Email is required if username is empty")
                    .EmailAddress()
                    .MustAsync(async (email, _) => !await predicates.IsUserEmailFree(email))
                    .WithMessage("User with this email doesn't exist");
            });

            When(x => string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Username)
                    .MinimumLength(3)
                    .MaximumLength(32)
                    .Matches(@"^[a-z0-9_]+$")
                    .WithMessage("Username must contain only lowercase alphanumeric characters and underscores")
                    .MustAsync(async (username, _) => !await predicates.IsUsernameFree(username))
                    .WithMessage("User with this username doesn't exist");
            });

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .MaximumLength(32)
                .Matches(@"^[a-zA-Z0-9_]+$")
                .WithMessage("Password must contain only alphanumeric characters and underscores");
        }
    }
}