using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public record GetProfilePhotoQuery(Guid ProfilePhotoId) : IQuery<Result<ProfilePhotoDto>>;