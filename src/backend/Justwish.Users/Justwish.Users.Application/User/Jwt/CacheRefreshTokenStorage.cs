using Justwish.Users.Domain;
using Microsoft.Extensions.Caching.Distributed;

namespace Justwish.Users.Application;

public sealed class CacheRefreshTokenStorage : IJwtRefreshTokenStorage
{
    private const string RefreshTokenPrefix = "RefreshToken:";
    private const string ValidStatus = "valid";
    // ReSharper disable once UnusedMember.Local
    // Maybe in the future: If invalidation is requested key won't be removed but value will be set to invalid
    private const string InvalidStatus = "invalid";  
    
    private readonly IDistributedCache _cache;

    public CacheRefreshTokenStorage(IDistributedCache cache)
    {
        _cache = cache;
    }
    
    public async Task<bool> IsValidAsync(JwtToken refreshToken)
    {
        string key = RefreshTokenPrefix + refreshToken.Token;
        var value = await _cache.GetStringAsync(key);
        
        return value is not null && value == ValidStatus;
    }
    
    public async Task RemoveAsync(JwtToken refreshToken)
    {
        string key = RefreshTokenPrefix + refreshToken.Token;
        
        await _cache.RemoveAsync(key);
    }

    public async Task StoreAsync(JwtToken refreshToken, TimeSpan expirationTime)
    {
        string key = RefreshTokenPrefix + refreshToken.Token;

        var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime };
        await _cache.SetStringAsync(key, ValidStatus, cacheOptions);
    }
}