using Justwish.Users.Domain;
using Microsoft.Extensions.Caching.Distributed;

namespace Justwish.Users.Application;

public sealed class CacheEmailVerificationService : IEmailVerificationService
{
    private readonly IDistributedCache _cache;

    public CacheEmailVerificationService(IDistributedCache cache)
    {
        _cache = cache;
    }
    
    public int IssueEmailVerificationCode(string email)
    {
        throw new NotImplementedException(); // NotImplementedException
    }

    public bool VerifyEmail(string email, int code)
    {
        throw new NotImplementedException();
    }
}