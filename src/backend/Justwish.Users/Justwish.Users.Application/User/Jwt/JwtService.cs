using System.Security.Claims;
using Ardalis.Result;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Justwish.Users.Application;

public sealed class JwtService : IJwtService
{
    private const string TokenTypeClaimName = "token_type";
    private const string AccessTokenType = "access";
    private const string RefreshTokenType = "refresh";
    
    private readonly IJwtEncoder _encoder;
    private readonly IJwtRefreshTokenStorage _refreshTokenStorage;
    private readonly IUserReadRepository _userReadRepository;
    private readonly ILogger<JwtService> _logger;
    private readonly JwtOptions _options;

    public JwtService(IJwtEncoder encoder, IJwtRefreshTokenStorage refreshTokenStorage, 
        IUserReadRepository userReadRepository, IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger)
    {
        _encoder = encoder;
        _refreshTokenStorage = refreshTokenStorage;
        _userReadRepository = userReadRepository;
        _logger = logger;
        _options = jwtOptions.Value;
    }
    
    public async Task<JwtTokenPair> IssueAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);
        
        await _refreshTokenStorage.StoreAsync(refreshToken, _options.RefreshTokenExpirationTime);
        
        return new JwtTokenPair(accessToken, refreshToken);
    }

    public async Task<Result<JwtTokenPair>> RefreshAsync(JwtToken refreshToken)
    {
        if (!await _refreshTokenStorage.IsValidAsync(refreshToken))
        {
            _logger.LogDebug("Refresh token is not valid. Refresh failed.");
            return Result.Invalid(new ValidationError("Invalid refresh token."));
        }
        
        await _refreshTokenStorage.RemoveAsync(refreshToken);

        var claims = _encoder.DecodeToken(refreshToken).ToList();
        var userId = Guid.Parse(claims.Single(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        long expirationTime = long.Parse(claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
        long currentTime = EpochTime.GetIntDate(DateTime.UtcNow);
        
        if (expirationTime < currentTime)
        {
            _logger.LogDebug("Refresh token is expired. Refresh failed.");
            return Result.Error("Token expired.");
        }
        
        var user = await _userReadRepository.GetUserByIdAsync(userId);
        if (!user.IsSuccess)
        {
            _logger.LogDebug("User with Id {UserId} not found. Refresh failed.", userId);
            return Result.NotFound("User not found.");
        }
        
        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken(user);
        _logger.LogDebug("Tokens refreshed for user with {UserId} id.", userId);
        
        await _refreshTokenStorage.StoreAsync(newRefreshToken, _options.RefreshTokenExpirationTime);
        
        return new JwtTokenPair(newAccessToken, newRefreshToken);
    }

    public async Task InvalidateRefreshTokenAsync(JwtToken refreshToken)
    {
        await _refreshTokenStorage.RemoveAsync(refreshToken);
    }
    
    private JwtToken GenerateAccessToken(User user)
    {
        Claim[] claims = 
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(TokenTypeClaimName, AccessTokenType),
        ];
        
        return _encoder.CreateToken(claims, _options.AccessTokenExpirationTime);
    }

    private JwtToken GenerateRefreshToken(User user)
    {
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(TokenTypeClaimName, RefreshTokenType),
        ];
        
        return _encoder.CreateToken(claims, _options.RefreshTokenExpirationTime);
    }
}