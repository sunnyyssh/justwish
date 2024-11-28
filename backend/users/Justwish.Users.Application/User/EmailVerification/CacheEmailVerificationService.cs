using Ardalis.Result;
using Justwish.Users.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Justwish.Users.Application;

public sealed class CacheEmailVerificationService : IEmailVerificationService
{
    private const string CodePrefix = "EmailVerificationCode_";
    private const string StatusPrefix = "EmailVerified_";
    
    private readonly IDistributedCache _cache;
    private readonly IVerificationCodeGenerator _generator;
    private readonly ILogger<CacheEmailVerificationService> _logger;
    private readonly EmailVerificationOptions _options;

    public CacheEmailVerificationService(IDistributedCache cache, IVerificationCodeGenerator generator, 
        IOptions<EmailVerificationOptions> options, ILogger<CacheEmailVerificationService> logger)
    {
        _cache = cache;
        _generator = generator;
        _logger = logger;
        _options = options.Value;
    }
    
    public async Task<Result<int>> IssueCodeAsync(string email)
    {
        int code = _generator.GenerateCode();

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.CodePersistenceSeconds)
        };
        string codeKey = CodePrefix + email;
        await _cache.SetStringAsync(codeKey, code.ToString(), cacheOptions);
        
        string statusKey = StatusPrefix + email;
        await _cache.SetStringAsync(statusKey, EmailVerificationStatus.WaitingForVerification.ToString(), cacheOptions);
        
        return code;
    }
    
    public async Task<bool> VerifyEmailAsync(string email, int code)
    {
        string codeKey = CodePrefix + email;
        string statusKey = StatusPrefix + email;
        var storedCode = await _cache.GetStringAsync(codeKey);

        if (string.IsNullOrEmpty(storedCode)) 
        {
            _logger.LogDebug("Code is not found in cache by \"{CodeKey}\" key", codeKey);
        }
        else
        {
            _logger.LogDebug("Code is found in cache by \"{CodeKey}\" key", codeKey);
        }

        bool verified = int.TryParse(storedCode, out int verificationCode) && verificationCode == code;
        if (!verified)
        {
            _logger.LogDebug("Code is not equal to code in cache by \"{CodeKey}\" key", codeKey);
            return false;
        }
        
        await _cache.RemoveAsync(codeKey);
        _logger.LogDebug("Code removed from cache by \"{CodeKey}\" key", codeKey);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.VerifiedPersistenceSeconds)
        };
        await _cache.SetStringAsync(statusKey, EmailVerificationStatus.Verified.ToString(), cacheOptions);
        _logger.LogDebug("Status set to \"Verified\" by \"{StatusKey}\" key", statusKey);
        return true;
    }

    public async Task<EmailVerificationStatus> GetStatusAsync(string email)
    {
        string statusKey = StatusPrefix + email;
        var status = await _cache.GetStringAsync(statusKey);
        return Enum.TryParse(status, out EmailVerificationStatus result) ? result : EmailVerificationStatus.None;
    }
}