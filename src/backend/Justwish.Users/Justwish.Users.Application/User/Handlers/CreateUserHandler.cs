using Ardalis.Result;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Justwish.Users.Application;

public sealed class CreateUserHandler : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IEmailVerificationChecker _verificationChecker;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPublishEndpoint _publisher;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(IUserRepository repository, IEmailVerificationChecker verificationChecker,
        IPasswordHasher passwordHasher, IPublishEndpoint publisher, ILogger<CreateUserHandler> logger)
    {
        _repository = repository;
        _verificationChecker = verificationChecker;
        _passwordHasher = passwordHasher;
        _publisher = publisher;
        _logger = logger;
    }
    
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var verificationStatus = await _verificationChecker.GetStatusAsync(request.Email);
        if (verificationStatus != EmailVerificationStatus.Verified)
        {
            _logger.LogInformation("Email {Email} is not verified but attempted to create user", request.Email);
            return Result.Error("Email is not verified");
        }
        
        string passwordHash = _passwordHasher.Hash(request.Password);
        
        User user = new()
        {
            Username = request.Username.ToLower(),
            Email = request.Email,
            PasswordHash = passwordHash,
        };

        var addResult = await _repository.AddAsync(user);

        if (!addResult.IsSuccess)
        {
            _logger.LogError("Failed to create user with {Username} username and {Email} email", user.Username,
                user.Email);
            return addResult;
        }

        _logger.LogInformation("{Username} {Email} user is created", user.Username, user.Email);
        
        var userCreatedEvent = new UserCreatedEvent(user.Id, user.Username, user.Email);
        await _publisher.Publish(userCreatedEvent, cancellationToken);
        
        return Result.Success(UserDto.FromDomain(user));
    }
}