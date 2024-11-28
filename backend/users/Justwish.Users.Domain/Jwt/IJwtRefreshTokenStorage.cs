namespace Justwish.Users.Domain;

public interface IJwtRefreshTokenStorage
{
    public Task<bool> IsValidAsync(JwtToken refreshToken);
    
    public Task RemoveAsync(JwtToken refreshToken);
    
    public Task StoreAsync(JwtToken refreshToken, TimeSpan expirationTime);
}