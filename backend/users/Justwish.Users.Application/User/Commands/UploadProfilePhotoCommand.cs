using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record UploadProfilePhotoCommand(Guid UserId, string ContentType, byte[] Data) 
    : ICommand<Result<Guid>>;
