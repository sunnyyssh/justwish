using FastEndpoints;
using Justwish.Users.Application;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.WebApi;

public class GetUserByIdEndpoint : Endpoint<GetUserByIdEndpoint.GetUserRequest, Results<Ok<GetUserResponse>, NotFound>>
{
    private readonly ISender _sender;

    public GetUserByIdEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Get("/{Id:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<GetUserResponse>, NotFound>> ExecuteAsync(GetUserRequest request, CancellationToken cancellationToken)
    {
        var result =await _sender.Send(new GetUserByIdQuery(request.Id));
        if (!result.IsSuccess)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(GetUserResponse.FromUserDto(result.Value));
    }

    public record GetUserRequest(Guid Id);
}
