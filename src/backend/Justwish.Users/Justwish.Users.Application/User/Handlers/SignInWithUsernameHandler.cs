using Ardalis.Result;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Justwish.Users.Application;

public sealed class SignInWithUsernameHandler : ICommandHandler<SignInWithUsernameCommand, Result<JwtTokenPair>>
{
    private readonly IUserReadRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<SignInWithUsernameHandler> _logger;

    public SignInWithUsernameHandler(IUserReadRepository userRepository, IJwtService jwtService, 
        IPasswordHasher passwordHasher, ILogger<SignInWithUsernameHandler> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<JwtTokenPair>> Handle(SignInWithUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUsernameAsync(request.Username, cancellationToken);
        if (!user.IsSuccess)
        {
            return Result.Error("User not found");
        }

        if (!_passwordHasher.Verify(request.Password, user.Value.PasswordHash))
        {
            return Result.Error("Wrong password");
        }

        var tokenPair = await _jwtService.IssueAsync(user.Value);
        _logger.LogInformation("Jwt token pair issued for {UserId}", user.Value.Id);
        
        return tokenPair;
    }
}