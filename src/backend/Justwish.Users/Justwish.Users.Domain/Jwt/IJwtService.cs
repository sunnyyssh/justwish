using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Domain;

public interface IJwtService
{
    Task<JwtTokenPair> IssueAsync(User user);
    
    Task<Result<JwtTokenPair>> RefreshAsync(JwtToken refreshToken);
    
    Task InvalidateRefreshTokenAsync(JwtToken refreshToken);
}
