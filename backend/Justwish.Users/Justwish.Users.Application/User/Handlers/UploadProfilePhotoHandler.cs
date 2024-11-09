using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed class UploadProfilePhotoHandler : ICommandHandler<UploadProfilePhotoCommand, Result<Guid>>
{
    private readonly IProfilePhotoRepository _photoRepository;
    private readonly IUserRepository _userRepository;

    public UploadProfilePhotoHandler(IProfilePhotoRepository photoRepository, IUserRepository userRepository)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.NotFound();
        }

        var photo = new ProfilePhoto
        {
            ContentType = request.ContentType,
            Data = request.Data,
        };

        var addPhotoResult = await _photoRepository.AddUserProfilePhotoAsync(photo);
        if (!addPhotoResult.IsSuccess)
        {
            return Result.NotFound();
        }
        
        user.Value.ProfilePhotoId = addPhotoResult.Value;
        var updateResult = await _userRepository.UpdateAsync(user.Value, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            return Result.NotFound();
        }

        return addPhotoResult;
    }
}
