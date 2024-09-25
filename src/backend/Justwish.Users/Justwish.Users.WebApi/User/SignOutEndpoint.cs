using FastEndpoints;
using Justwish.Users.Application;
using Justwish.Users.Domain;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public sealed class SignOutEndpoint : Endpoint<SignOutEndpoint.SignOutRequest, Ok>
{
    private readonly ISender _sender;

    public SignOutEndpoint(ISender sender)
    {
        _sender = sender;
    }
    
    public override void Configure()
    {
        Post("auth/signout");
        AllowAnonymous();
    }

    public override async Task<Ok> ExecuteAsync(SignOutRequest req, CancellationToken ct)
    {
        // Nobody will know if it is even a token or not.
        // Maybe in the future there will be a need in knowing it. Now it is always OK.
        await _sender.Send(new SignOutCommand(new JwtToken(req.RefreshToken)), ct); 
        return TypedResults.Ok();
    }

    public record SignOutRequest(string RefreshToken);
}