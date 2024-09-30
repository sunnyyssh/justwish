using Ardalis.Result;

namespace Justwish.Users.Domain;

public interface IProfilePhotoRepository : IProfilePhotoReadRepository
{
    public Task<Result> AddUserProfilePhotoAsync(ProfilePhoto profilePhoto);

    public Task<Result> AddSharedAsync(ProfilePhoto profilePhoto);

    public Task<Result> DeleteAsync(ProfilePhoto profilePhoto);
}
