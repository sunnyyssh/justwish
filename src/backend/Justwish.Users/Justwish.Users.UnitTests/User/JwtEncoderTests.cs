using System.Security.Claims;
using Justwish.Users.Application;
using Justwish.Users.Domain;

namespace Justwish.Users.UnitTests;

public sealed class JwtEncoderTests
{
    [Fact]
    public void Decodes_Encoded()
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
        var decodedClaims = encoder.DecodeToken(jwtToken);

        // Assert
        Assert.Contains(decodedClaims, c => c is { Type: "test", Value: "it-is-correct" });
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