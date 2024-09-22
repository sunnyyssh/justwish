namespace Justwish.Users.WebApi.ApiKeyAuth;

public sealed class ApiKeyConstants
{
    public const string PolicyName = "ApiKey";
        
    public const string HeaderName = "x-api-key";
    
    public const string ConfigurationSection = "ApiKey";
    
    public const string AuthenticationScheme = "ApiKey";
    
    public const string ClaimType = "ApiClaim";
    
    public const string DefaultClaimValue = "Default";
}