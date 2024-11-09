using FastEndpoints;
using Justwish.Users.Application;
using MassTransit.Futures.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Justwish.Users.WebApi;

public sealed class GetProfilePhotoEndpoint 
    : Endpoint<GetProfilePhotoEndpoint.PhotoRequest, Results<FileContentHttpResult, NotFound>>
{
    private const string PhotoIdQueryParam = "PhotoId";

    private readonly ISender _sender;

    public GetProfilePhotoEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Get($"/profile-photo/{{{PhotoIdQueryParam}:guid}}");
        //Throttle(100, 60); // File sending can be exhausting
        AllowAnonymous();
    }

    public override async Task<Results<FileContentHttpResult, NotFound>> ExecuteAsync(PhotoRequest req, CancellationToken ct)
    {
        var result = await _sender.Send(new GetProfilePhotoQuery(req.PhotoId), ct);

        return result.IsSuccess
            ? TypedResults.File(result.Value.Data, result.Value.ContentType)
            : TypedResults.NotFound();
    }

    public record PhotoRequest([property: FromRoute(Name = PhotoIdQueryParam)] Guid PhotoId);
}
