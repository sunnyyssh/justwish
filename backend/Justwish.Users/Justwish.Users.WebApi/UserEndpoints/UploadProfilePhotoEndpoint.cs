using System;
using System.IdentityModel.Tokens.Jwt;
using FastEndpoints;
using FluentValidation;
using Justwish.Users.Application;
using MediatR;

namespace Justwish.Users.WebApi;

public class UploadProfilePhotoEndpoint 
    : Endpoint<UploadProfilePhotoEndpoint.UploadRequest, UploadProfilePhotoEndpoint.UploadResponse>
{
    private static readonly IReadOnlyList<string> AcceptableContentTypes =
        ["image/jpg", "image/jpeg", "image/png", "image/gif", "image/webp"];

    private const int MaxFileSize = 5 * 1024 * 1024;

    private readonly ISender _sender;

    public UploadProfilePhotoEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("/profile-photo/upload");
        Policies(AuthConstants.ValidJwtTokenTypePolicy);
        Validator<UploadRequestValidator>();
        AllowFileUploads();
    }

    public override async Task<UploadResponse> ExecuteAsync(UploadRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

        var stream = req.File.OpenReadStream();
        byte[] bytes = new byte[req.File.Length];

        await stream.ReadAsync(bytes, ct);

        var uploadPhotoCommand = new UploadProfilePhotoCommand(userId, req.File.ContentType, bytes);
        var result = await _sender.Send(uploadPhotoCommand);

        return new UploadResponse(result.Value);    
    }

    public record UploadRequest(IFormFile File);

    public record UploadResponse(Guid PhotoId);

    public sealed class UploadRequestValidator : Validator<UploadRequest>
    {
        public UploadRequestValidator()
        {
            RuleFor(x => x.File)
                .NotEmpty()
                .Must(f => AcceptableContentTypes.Contains(f.ContentType))
                .WithMessage("Only JPEG, PNG, GIF and WebP images are allowed");
        }
    }
}
