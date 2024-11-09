using Ardalis.Result;

namespace Justwish.Users.Domain;

public interface IProfilePhotoRepository : IProfilePhotoReadRepository
{
    public Task<Result<Guid>> AddUserProfilePhotoAsync(ProfilePhoto profilePhoto);

    public Task<Result<Guid>> AddSharedAsync(ProfilePhoto profilePhoto);

    public Task<Result> DeleteAsync(ProfilePhoto profilePhoto);
}
