using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed class GetProfilePhotoHandler : IQueryHandler<GetProfilePhotoQuery, Result<ProfilePhotoDto>>
{
    private readonly IProfilePhotoReadRepository _repository;

    public GetProfilePhotoHandler(IProfilePhotoReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProfilePhotoDto>> Handle(GetProfilePhotoQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetProfilePhotoByIdAsync(request.ProfilePhotoId);

        if (!result.IsSuccess)
        {
            return Result.NotFound();
        }

        var photoDto = new ProfilePhotoDto 
        {
            Id = result.Value.Id,
            ContentType = result.Value.ContentType,
            Data = result.Value.Data,
        };
        return Result.Success(photoDto);
    }
}
