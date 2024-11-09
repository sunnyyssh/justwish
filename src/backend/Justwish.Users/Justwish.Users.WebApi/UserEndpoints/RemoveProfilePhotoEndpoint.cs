using System;
using System.IdentityModel.Tokens.Jwt;
using FastEndpoints;
using Justwish.Users.Application;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public class RemoveProfilePhotoEndpoint : EndpointWithoutRequest<NoContent>
{
    private readonly ISender _sender;

    public RemoveProfilePhotoEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Delete("/profile-photo/remove");
        Policies(AuthConstants.ValidJwtTokenTypePolicy);
    }

    public override async Task<NoContent> ExecuteAsync(CancellationToken ct)
    {
        Guid userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        _ = await _sender.Send(new RemoveProfilePhotoCommand(userId), ct);

        return TypedResults.NoContent();
    }
}
