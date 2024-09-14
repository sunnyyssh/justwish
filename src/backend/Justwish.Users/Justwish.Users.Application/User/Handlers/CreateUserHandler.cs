using Ardalis.Result;
using Justwish.Users.Application.Commands;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed class CreateUserHandler : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    public CreateUserHandler()
    {
        
    }
    
    public Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}