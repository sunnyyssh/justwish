using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public class DeleteUserHandler : ICommandHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        if (!user.IsSuccess)
        {
            return Result.NotFound();
        }

        await _userRepository.DeleteAsync(user, cancellationToken);

        return Result.Success();
    }
}
