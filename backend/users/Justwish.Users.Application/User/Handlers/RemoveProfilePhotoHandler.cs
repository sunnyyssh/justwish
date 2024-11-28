using System;
using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed class RemoveProfilePhotoHandler : ICommandHandler<RemoveProfilePhotoCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IProfilePhotoReadRepository _photoRepository;

    public RemoveProfilePhotoHandler(IUserRepository userRepository, IProfilePhotoReadRepository photoRepository)
    {
        _userRepository = userRepository;
        _photoRepository = photoRepository;
    }

    public async Task<Result> Handle(RemoveProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (!user.IsSuccess)
        {
            return Result.NotFound();
        }

        // Just replace the PhotoId. Previous photo is still in the database.
        // To decrease stored data size remove photo if it is not shared.
        user.Value.ProfilePhotoId = await _photoRepository.GetRandomSharedPhotoIdAsync(cancellationToken);
        await _userRepository.UpdateAsync(user.Value, cancellationToken);

        return Result.Success();
    }
}
