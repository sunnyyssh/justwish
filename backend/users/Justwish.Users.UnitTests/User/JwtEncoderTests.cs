using System.Security.Claims;
using Justwish.Users.Application;
using Justwish.Users.Domain;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Justwish.Users.UnitTests;

public sealed class JwtEncoderTests
{
    [Theory]
    [InlineData("test", "it-is-test-claim-data")]
    [InlineData(JwtRegisteredClaimNames.Sub, "1253a83a-a5b4-4afc-88ac-af65084ac5fc")]
    public void Decodes_Encoded(string claimType, string claimValue)
    {
        // Arrange
        var optionsMock = MockHelpers.MockOptions(new JwtOptions
        {
            Issuer = "Issuer",
            Audience = "Audience",
            AccessTokenExpirationTime = TimeSpan.FromMinutes(5),
            RefreshTokenExpirationTime = TimeSpan.FromDays(7),
            SecretKey = "F469CCC469474976AD764A73B57A3B18",
        });
        var encoder = new JwtEncoder(optionsMock.Object);

        Claim[] testClaims = [new Claim(claimType, claimValue)];
        
        // Act
        var jwtToken = encoder.CreateToken(testClaims, optionsMock.Object.Value.AccessTokenExpirationTime);
        var decodedClaims = encoder.DecodeToken(jwtToken);

        // Assert
        Assert.Contains(decodedClaims, c => c.Type == claimType && c.Value == claimValue);
    }
    
    [Fact]
    public void Validates_Encoded()
    {
        // Arrange
        var optionsMock = MockHelpers.MockOptions(new JwtOptions
        {
            Issuer = "Issuer",
            Audience = "Audience",
            AccessTokenExpirationTime = TimeSpan.FromMinutes(5),
            RefreshTokenExpirationTime = TimeSpan.FromDays(7),
            SecretKey = "F469CCC469474976AD764A73B57A3B18",
        });
        var encoder = new JwtEncoder(optionsMock.Object);

        Claim[] testClaims = [new Claim("test", "it-is-correct")];
        
        // Act
        var jwtToken = encoder.CreateToken(testClaims, optionsMock.Object.Value.AccessTokenExpirationTime);

        // Assert
        encoder.ValidateToken(jwtToken);
    }
    
    [Fact]
    public void Doesnt_Validate_Not_Encoded()
    {
        // Arrange
        var fakeToken = new JwtToken(@"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
        
        var optionsMock = MockHelpers.MockOptions(new JwtOptions
        {
            Issuer = "Issuer",
            Audience = "Audience",
            AccessTokenExpirationTime = TimeSpan.FromMinutes(5),
            RefreshTokenExpirationTime = TimeSpan.FromDays(7),
            SecretKey = "F469CCC469474976AD764A73B57A3B18",
        });
        var encoder = new JwtEncoder(optionsMock.Object);

        Claim[] testClaims = [new Claim("test", "it-is-correct")];
        
        // Act
        _ = encoder.CreateToken(testClaims, optionsMock.Object.Value.AccessTokenExpirationTime);

        // Assert
        Assert.ThrowsAny<Exception>(() => encoder.ValidateToken(fakeToken));
    }
}