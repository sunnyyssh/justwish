using System;
using System.IdentityModel.Tokens.Jwt;
using FastEndpoints;
using FluentValidation;
using Justwish.Users.Application;
using Justwish.Users.Domain;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public sealed class UpdateUserProfileEndpoint : Endpoint<UpdateUserProfileEndpoint.UpdateRequest, Results<NoContent, BadRequest<string>>>
{
    private readonly ISender _sender;

    public UpdateUserProfileEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Put("/profile/");
        Policies(AuthConstants.ValidJwtTokenTypePolicy);
        Validator<UpdateRequestValidator>();
    }

    public override async Task<Results<NoContent, BadRequest<string>>> ExecuteAsync(UpdateRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        var updateCommand = new UpdateUserProfileCommand(
            userId,
            req.FirstName,
            req.LastName,
            req.DateOfBirth,
            req.Gender,
            req.SocialLinks);
        
        var result = await _sender.Send(updateCommand);
        return result.IsSuccess ? TypedResults.NoContent() : TypedResults.BadRequest(string.Join(", ", result.Errors));
    }

    // ATTENTION: null values in the request mean that the property should be removed from the user
    public sealed record UpdateRequest(
        string? FirstName,
        string? LastName,
        DateOnly? DateOfBirth,
        Gender? Gender,
        List<string>? SocialLinks);

    public sealed class UpdateRequestValidator : Validator<UpdateRequest>
    {
        public UpdateRequestValidator()
        {
            When(x => x.FirstName is not null, () =>
                RuleFor(x => x.FirstName!)
                    .MaximumLength(50)
                    .Matches("^[A-Za-z]+[' -]?[A-Za-z]+$"));

            When(x => x.LastName is not null, () =>
                RuleFor(x => x.LastName!)
                    .MaximumLength(50)
                    .Matches("^[A-Za-z]+[' -]?[A-Za-z]+$"));

            When(x => x.DateOfBirth is not null, () =>
                RuleFor(x => x.DateOfBirth!)
                    .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)));

            When(x => x.Gender is not null, () =>
                RuleFor(x => x.Gender!)
                    .Must(x => x is Gender.Male or Gender.Female)
                    .WithMessage("Gender must be either Male or Female"));

            When(x => x.SocialLinks is not null, () =>
                RuleFor(x => x.SocialLinks!)
                    .Must(x => x.Count <= 5)
                    .WithMessage("You can only add up to 5 social links")
                    .Must(x => x.All(link => link.Length <= 50))
                    .WithMessage("Social links must be less than 50 characters")
                    .Must(x => x.All(link => link.StartsWith("https://") || link.StartsWith("http://")))
                    .WithMessage("Social links must start with https:// or http://"));
        }
    }
}
