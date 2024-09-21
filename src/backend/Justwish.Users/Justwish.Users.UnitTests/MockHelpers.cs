using Justwish.Users.Application;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;

namespace Justwish.Users.UnitTests;

public static class MockHelpers
{
    public static Mock<IDistributedCache> MockCacheWithDict(IDictionary<string, byte[]> dict)
    {
        var mock = new Mock<IDistributedCache>();
        
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        mock.Setup(c => 
                c.SetAsync(
                    It.IsAny<string>(), 
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()))
            .Returns(async (string key, byte[] value, DistributedCacheEntryOptions _, CancellationToken _) => dict[key] = value);
        
        mock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(async (string key, CancellationToken _) => dict.TryGetValue(key, out var res) ? res : null);
        
        mock.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(async (string key, CancellationToken _) => dict.Remove(key));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        
        return mock;
    }

    public static Mock<IOptions<TOptions>> MockOptions<TOptions>(TOptions options)
        where TOptions : class
    {
        var mock = new Mock<IOptions<TOptions>>();
        mock.SetupGet(c => c.Value).Returns(options);
        return mock;
    }
}