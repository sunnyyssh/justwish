using Ardalis.Result;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using MassTransit;

namespace Justwish.Users.Application;

public sealed class CreateUserHandler : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPublishEndpoint _publisher;

    public CreateUserHandler(IUserRepository repository, IPasswordHasher passwordHasher, IPublishEndpoint publisher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _publisher = publisher;
    }
    
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        string passwordHash = _passwordHasher.Hash(request.Password);
        
        User user = new()
        {
            Username = request.Email,
            Email = request.Email,
            PasswordHash = passwordHash,
        };

        var addResult = await _repository.AddAsync(user);

        if (!addResult.IsSuccess)
        {
            return addResult;
        }

        var userCreatedEvent = new UserCreatedEvent(user.Id, user.Username, user.Email);
        await _publisher.Publish(userCreatedEvent, cancellationToken);
        
        return Result.Success(UserDto.FromDomain(user));
    }
}