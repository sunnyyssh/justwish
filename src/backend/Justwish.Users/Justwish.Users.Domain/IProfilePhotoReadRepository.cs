using Ardalis.Result;

namespace Justwish.Users.Domain;

public interface IProfilePhotoReadRepository
{
    public Task<List<(Guid id, string alias)>> ListAllSharedAsync();

    public Task<Result<ProfilePhoto>> GetProfilePhotoByIdAsync(Guid id);

    public Task<Result<Guid>> GetRandomSharedPhotoIdAsync(CancellationToken cancellationToken = default);

    public Task<Result<ProfilePhoto>> GetSharedByAliasAsync(string alias);
}