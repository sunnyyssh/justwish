using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public class UpdateUserProfileHandler : ICommandHandler<UpdateUserProfileCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken)
                is not { Value: { } user })
        {
            return Result.NotFound();
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DateOfBirth = request.DateOfBirth;
        user.Gender = request.Gender;
        user.SocialLinks = request.SocialLinks;
        
        var result = await _userRepository.UpdateAsync(user, cancellationToken);

        return result;
    }
}
