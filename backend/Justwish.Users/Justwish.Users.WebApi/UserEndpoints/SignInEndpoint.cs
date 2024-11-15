﻿using Ardalis.Result;
using FastEndpoints;
using FluentValidation;
using Justwish.Users.Application;
using Justwish.Users.Domain;
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
        public SignInValidator()
        {
            When(x => string.IsNullOrWhiteSpace(x.Username), () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Email is required if username is empty")
                    .EmailAddress()
                    .MustAsync(async (email, _) =>
                    {
                        var rulePredicates = Resolve<IUserBusinessRulePredicates>();
                        return !await rulePredicates.IsUserEmailFree(email);
                    })
                    .WithMessage("User with this email doesn't exist");
            });

            When(x => string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Username)
                    .Username()
                    .MustAsync(async (username, _) =>
                    {
                        var rulePredicates = Resolve<IUserBusinessRulePredicates>();
                        return !await rulePredicates.IsUsernameFree(username);
                    })
                    .WithMessage("User with this username doesn't exist");
            });

            RuleFor(x => x.Password)
                .Password();
        }
    }
}