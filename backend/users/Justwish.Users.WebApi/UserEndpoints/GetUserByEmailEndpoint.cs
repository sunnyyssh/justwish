using System;
using FastEndpoints;
using FluentValidation;
using Justwish.Users.Application;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public class GetUserByEmailEndpoint : Endpoint<GetUserByEmailEndpoint.GetUserRequest, Results<Ok<GetUserResponse>, NotFound>>
{
    private readonly ISender _sender;

    public GetUserByEmailEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Get("/email/{Email}");
        AllowAnonymous();
        Validator<GetUserRequestValidator>();
    }

    public override async Task<Results<Ok<GetUserResponse>, NotFound>> ExecuteAsync(GetUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserByEmailQuery(request.Email));
        if (!result.IsSuccess)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(GetUserResponse.FromUserDto(result.Value));
    }

    public record GetUserRequest(string Email);

    class GetUserRequestValidator : Validator<GetUserRequest>
    {
        public GetUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();
        }
    }
}

