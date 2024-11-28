namespace Justwish.Users.Application;

public sealed class JwtOptions
{
    public TimeSpan AccessTokenExpirationTime { get; set; } = TimeSpan.FromMinutes(5);

    public TimeSpan RefreshTokenExpirationTime { get; set; } = TimeSpan.FromDays(7);
    
    public string SecretKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;
    
    
}