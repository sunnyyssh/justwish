using FastEndpoints;
using Justwish.Users.Application;
using Justwish.Users.Domain;
using Justwish.Users.WebApi.ApiKeyAuth;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public sealed class RefreshEndpoint : Endpoint<RefreshEndpoint.RefreshRequest, Results<Ok<RefreshEndpoint.RefreshResponse>, BadRequest<string>>>
{
    private readonly ISender _sender;

    public RefreshEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("auth/refresh");
        Policies(ApiKeyConstants.PolicyName);
    }

    public override async Task<Results<Ok<RefreshResponse>, BadRequest<string>>> ExecuteAsync(RefreshRequest req, CancellationToken ct)
    {
        var result = await _sender.Send(new RefreshCommand(new JwtToken(req.RefreshToken)), ct);
        return result.IsSuccess 
            ? TypedResults.Ok(new RefreshResponse(result.Value.AccessToken.Token, result.Value.RefreshToken.Token))
            : TypedResults.BadRequest(string.Join("; ", result.Errors));
    }

    public record RefreshRequest(string RefreshToken);
    
    public sealed record RefreshResponse(string AccessToken, string RefreshToken);
}