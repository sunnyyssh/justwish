using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Justwish.Users.Application;

public sealed class CreateUserHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _repository;
    private readonly IEmailVerificationChecker _verificationChecker;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPublishEndpoint _publisher;
    private readonly ILogger<CreateUserHandler> _logger;
    private readonly IProfilePhotoReadRepository _photoRepository;

    public CreateUserHandler(IUserRepository repository, IProfilePhotoReadRepository photoRepository, 
        IEmailVerificationChecker verificationChecker, IPasswordHasher passwordHasher, 
        IPublishEndpoint publisher, ILogger<CreateUserHandler> logger)
    {
        _repository = repository;
        _photoRepository = photoRepository;
        _verificationChecker = verificationChecker;
        _passwordHasher = passwordHasher;
        _publisher = publisher;
        _logger = logger;
    }
    
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var verificationStatus = await _verificationChecker.GetStatusAsync(request.Email);
        if (verificationStatus != EmailVerificationStatus.Verified)
        {
            _logger.LogInformation("Email {Email} is not verified but attempted to create user", request.Email);
            return CreateUserResponse.EmailNotVerified();
        }
        
        string passwordHash = _passwordHasher.Hash(request.Password);
        
        User user = new()
        {
            Username = request.Username.ToLower(),
            Email = request.Email,
            PasswordHash = passwordHash,
            ProfilePhotoId = await _photoRepository.GetRandomSharedPhotoIdAsync(cancellationToken)
        };

        var addResult = await _repository.AddAsync(user);

        if (!addResult.IsSuccess)
        {
            _logger.LogError("Failed to create user with {Username} username and {Email} email", user.Username,
                user.Email);
            throw new InvalidOperationException("Can't create user");
        }

        _logger.LogInformation("New user is created: \"{Username}\" username and \"{Email}\" email", user.Username, user.Email);
        
        var userCreatedEvent = new UserCreatedEvent(user.Id, user.Username, user.Email);
        await _publisher.Publish(userCreatedEvent, cancellationToken);
        _logger.LogInformation("UserCreatedEvent published: {Event}", userCreatedEvent);
        
        return CreateUserResponse.Success(UserDto.FromDomain(user));
    }
}