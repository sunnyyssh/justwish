using Justwish.Users.Application;
using Justwish.Users.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Justwish.Users.UnitTests;

public sealed class CacheRefreshTokenStorageTests
{
    [Fact]
    public async Task Doesnt_Validate_NotStored_RefreshToken()
    {
        // Arrange
        var jwtToken = new JwtToken(@"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
        
        var cacheMock = MockHelpers.MockCacheWithDict(new Dictionary<string, byte[]>());
        var storage = new CacheRefreshTokenStorage(cacheMock.Object);
        
        // Act
        bool valid = await storage.IsValidAsync(jwtToken);

        //Assert
        Assert.False(valid);
    }
    
    [Fact]
    public async Task Validates_Stored_RefreshToken()
    {
        // Arrange
        var jwtToken = new JwtToken(@"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
        
        var cacheMock = MockHelpers.MockCacheWithDict(new Dictionary<string, byte[]>());
        var storage = new CacheRefreshTokenStorage(cacheMock.Object);
        
        // Act
        await storage.StoreAsync(jwtToken, TimeSpan.FromHours(1));
        bool valid = await storage.IsValidAsync(jwtToken);

        //Assert
        Assert.True(valid);
    }
    
    [Fact]
    public async Task Doesnt_Validate_Removed_RefreshToken()
    {
        // Arrange
        var jwtToken = new JwtToken(@"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
        
        var cacheMock = MockHelpers.MockCacheWithDict(new Dictionary<string, byte[]>());
        var storage = new CacheRefreshTokenStorage(cacheMock.Object);
        
        // Act
        await storage.StoreAsync(jwtToken, TimeSpan.FromHours(1));
        await storage.RemoveAsync(jwtToken);
        bool valid = await storage.IsValidAsync(jwtToken);

        //Assert
        Assert.False(valid);
    }
}