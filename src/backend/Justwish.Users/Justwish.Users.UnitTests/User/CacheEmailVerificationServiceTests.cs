using Justwish.Users.Application;
using Justwish.Users.Domain;
using MassTransit.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Justwish.Users.UnitTests;

public sealed class CacheEmailVerificationServiceTests
{
    [Fact]
    public async Task Issues_GeneratedCode()
    {
        // Arrange
        const int trueCode = 6969;
        const string email = "test@test.com";
        
        var service = CreateWithMocks(trueCode, new Dictionary<string, byte[]>(), 4);

        // Act
        int issuedCode = await service.IssueCodeAsync(email);
        
        // Assert
        Assert.Equal(trueCode, issuedCode);
    }

    [Fact]
    public async Task Verifies_IssuedCode()
    {
        // Arrange
        const int trueCode = 6969;
        const string email = "test@test.com";
        
        var service = CreateWithMocks(trueCode, new Dictionary<string, byte[]>(), 4);

        // Act
        int issuedCode = await service.IssueCodeAsync(email);
        bool verified = await service.VerifyEmailAsync(email, issuedCode);

        // Assert
        Assert.True(verified);
    }

    [Fact]
    public async Task Doesnt_Verify_NonIssuedCode()
    {
        // Arrange
        const int trueCode = 6969;
        const int wrongCode = 4242;
        const string email = "test@test.com";

        var service = CreateWithMocks(trueCode, new Dictionary<string, byte[]>(), 4);

        // Act
        int _ = await service.IssueCodeAsync(email);
        bool verified = await service.VerifyEmailAsync(email, wrongCode);

        // Assert
        Assert.False(verified);
    }

    [Fact]
    public async Task Returns_Verified_Status()
    {
        // Arrange
        const string email = "test@test.com";
        
        var service = CreateWithMocks(6969, new Dictionary<string, byte[]>(), 4);

        // Act
        int issuedCode = await service.IssueCodeAsync(email);
        _ = await service.VerifyEmailAsync(email, issuedCode);
        var status = await service.GetStatusAsync(email);
        
        // Assert
        Assert.Equal(EmailVerificationStatus.Verified, status);
    }
    
    private static CacheEmailVerificationService CreateWithMocks(int generatingCode,
        Dictionary<string, byte[]> cacheStore, int codeLength)
    {
        var codeGeneratorMock = MockCodeGeneratorReturns(generatingCode);
        var cacheMock = MockHelpers.MockCacheWithDict(cacheStore);
        var optionsMock = MockHelpers.MockOptions(new EmailVerificationOptions { CodeLength = codeLength });
        var loggerMock = new Mock<ILogger<CacheEmailVerificationService>>();

        return new CacheEmailVerificationService(cacheMock.Object, codeGeneratorMock.Object, optionsMock.Object,
            loggerMock.Object);
    }

    private static Mock<IVerificationCodeGenerator> MockCodeGeneratorReturns(int code)
    {
        var mock = new Mock<IVerificationCodeGenerator>();
        mock.Setup(c => c.GenerateCode()).Returns(code);
        return mock;
    }
}