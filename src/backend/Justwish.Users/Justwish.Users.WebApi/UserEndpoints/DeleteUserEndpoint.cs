using System;
using System.IdentityModel.Tokens.Jwt;
using FastEndpoints;
using Justwish.Users.Application;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public class DeleteUserEndpoint : EndpointWithoutRequest<NoContent>
{
    private readonly ISender _sender;

    public DeleteUserEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Delete("/account");
        Policies(AuthConstants.ValidJwtTokenTypePolicy);
    }

    public override async Task<NoContent> ExecuteAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

        var deleteAccountCommand = new DeleteUserCommand(userId);
        _ = await _sender.Send(deleteAccountCommand, ct);

        return TypedResults.NoContent();
    }
}
