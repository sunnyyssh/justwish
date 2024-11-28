using FastEndpoints;
using Justwish.Users.Application;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public class GetUserByUsernameEndpoint : Endpoint<GetUserByUsernameEndpoint.GetUserRequest, Results<Ok<GetUserResponse>, NotFound>>
{
    private readonly ISender _sender;

    public GetUserByUsernameEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Get("/username/{Username}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<GetUserResponse>, NotFound>> ExecuteAsync(GetUserRequest req, CancellationToken ct)
    {
        var result = await _sender.Send(new GetUserByUsernameQuery(req.Username), ct);
        if (!result.IsSuccess)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(GetUserResponse.FromUserDto(result.Value));
    }

    public record GetUserRequest(string Username);

    public class GetUserRequestValidator : Validator<GetUserRequest>
    {
        public GetUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .Username();
        }
    }
}
