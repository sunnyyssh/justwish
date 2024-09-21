using System.Security.Claims;
using Justwish.Users.Application;
using Justwish.Users.Domain;
using Microsoft.Extensions.Options;
using Moq;

namespace Justwish.Users.UnitTests;

public sealed class JwtServiceTests
{
    private static readonly User TestUser = new("peter", "peter@peter.ptr", "HASH1HASH3HASH5HASH6HASH9HASH");

    [Fact]
    public async Task Issues_AndStores()
    {
        // Arrange
        var mockStorage = MockStorage();
        var options = CreateMockOptions();
        
        var service = new JwtService(
            MockJwtEncoder().Object, 
            mockStorage.Object, 
            new MockUserRepository([TestUser]), 
            options);
        
        // Act
        var (_, refreshToken) = await service.IssueAsync(TestUser);

        // Assert
        mockStorage.Verify(x => x.StoreAsync(refreshToken, options.Value.RefreshTokenExpirationTime), Times.Once);
    }

    [Fact]
    public async Task Refreshes_And_Rotates_WithIssuedToken()
    {
        // Arrange
        var mockStorage = MockStorage();
        var options = CreateMockOptions();
        
        var service = new JwtService(
            MockJwtEncoder().Object, 
            mockStorage.Object, 
            new MockUserRepository([TestUser]), 
            options);
        
        // Act
        var issued = await service.IssueAsync(TestUser);
        var refreshed = await service.RefreshAsync(issued.RefreshToken);
        
        // Assert
        Assert.True(refreshed.IsSuccess);
        Assert.NotEqual(issued.AccessToken, refreshed.Value.AccessToken);
        Assert.NotEqual(issued.RefreshToken, refreshed.Value.RefreshToken);
    }

    [Fact]
    public async Task Doesnt_Refresh_With_WrongToken()
    {
        // Arrange
        var mockStorage = MockStorage();
        var options = CreateMockOptions();
        
        var service = new JwtService(
            MockJwtEncoder().Object, 
            mockStorage.Object, 
            new MockUserRepository([TestUser]), 
            options);
        
        // Act
        var (access, _) = await service.IssueAsync(TestUser);
        var refreshed = await service.RefreshAsync(access); // Must be refresh token.
        
        // Assert
        Assert.False(refreshed.IsSuccess);
    }
    
    [Fact]
    public async Task Removes_WhenInvalidating()
    {
        // Arrange
        var mockStorage = MockStorage();
        var options = CreateMockOptions();
        
        var service = new JwtService(
            MockJwtEncoder().Object, 
            mockStorage.Object, 
            new MockUserRepository([TestUser]), 
            options);

        var jwtToken = new JwtToken("It_is_real_token_I_swear.(No).");
        await mockStorage.Object.StoreAsync(jwtToken, options.Value.RefreshTokenExpirationTime);
        
        // Act
        await service.InvalidateRefreshTokenAsync(jwtToken);

        // Assert
        mockStorage.Verify(x => x.RemoveAsync(jwtToken), Times.Once);
    }
    
    
    
    private static Mock<IJwtRefreshTokenStorage> MockStorage()
    {
        var mock = new Mock<IJwtRefreshTokenStorage>();
        var set = new HashSet<string>();
        
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        mock.Setup(x => x.StoreAsync(It.IsAny<JwtToken>(), It.IsAny<TimeSpan>()))
            .Returns(async (JwtToken token, TimeSpan _) => set.Add(token.Token));
        
        mock.Setup(x => x.RemoveAsync(It.IsAny<JwtToken>()))
            .Returns(async (JwtToken token) => set.Remove(token.Token));
        
        mock.Setup(x => x.IsValidAsync(It.IsAny<JwtToken>()))
            .Returns(async (JwtToken token) => set.Contains(token.Token));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        
        return mock;
    }
    
    private static Mock<IJwtEncoder> MockJwtEncoder()
    {
        // It stores claims like it would be stored in encoded token. The token will be just Guid that is key to this dict.
        var secretDictionary = new Dictionary<string, IEnumerable<Claim>>();
        
        var mock = new Mock<IJwtEncoder>();
        mock.Setup(x => x.CreateToken(It.IsAny<IEnumerable<Claim>>(), It.IsAny<TimeSpan>()))
            .Returns((IEnumerable<Claim> claims, TimeSpan exp) =>
            {
                var imagineItIsToken = Guid.NewGuid().ToString();
                secretDictionary[imagineItIsToken] = claims;
                return new JwtToken(imagineItIsToken);
            });

        mock.Setup(x => x.DecodeToken(It.IsAny<JwtToken>()))
            .Returns((JwtToken jwtToken) =>
                secretDictionary.TryGetValue(jwtToken.Token, out var claims) ? claims : []);

        mock.Setup(x => x.ValidateToken(It.IsAny<JwtToken>()))
            .Callback((JwtToken jwtToken) =>
            {
                if (!secretDictionary.TryGetValue(jwtToken.Token, out _))
                {
                    throw new Exception("Invalid token");
                }
            });

        return mock;
    }
    
    private static IOptions<JwtOptions> CreateMockOptions()
    {
        return MockHelpers.MockOptions(new JwtOptions
        {
            Issuer = "Issuer",
            Audience = "Audience",
            AccessTokenExpirationTime = TimeSpan.FromMinutes(5),
            RefreshTokenExpirationTime = TimeSpan.FromDays(7),
            SecretKey = "F469CCC469474976AD764A73B57A3B18",
        }).Object;
    }
}