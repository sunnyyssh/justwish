using Justwish.Users.Application;
using Justwish.Users.Domain;
using MassTransit.Configuration;
using Microsoft.Extensions.Caching.Distributed;
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
        var cacheMock = MockCacheWithDict(cacheStore);
        var optionsMock = MockOptions(new EmailVerificationOptions { CodeLength = codeLength });
        
        return new CacheEmailVerificationService(cacheMock.Object, codeGeneratorMock.Object, optionsMock.Object);
    }

    private static Mock<IVerificationCodeGenerator> MockCodeGeneratorReturns(int code)
    {
        var mock = new Mock<IVerificationCodeGenerator>();
        mock.Setup(c => c.GenerateCode()).Returns(code);
        return mock;
    }

    private static Mock<IDistributedCache> MockCacheWithDict(IDictionary<string, byte[]> dict)
    {
        var mock = new Mock<IDistributedCache>();
        
        mock.Setup(c => 
                c.SetAsync(
                    It.IsAny<string>(), 
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()))
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            .Returns(async (string key, byte[] value, DistributedCacheEntryOptions _, CancellationToken _) => dict[key] = value);
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        
        mock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string key, CancellationToken _) => dict[key]);
        return mock;
    }

    private static Mock<IOptions<EmailVerificationOptions>> MockOptions(EmailVerificationOptions options)
    {
        var mock = new Mock<IOptions<EmailVerificationOptions>>();
        mock.SetupGet(c => c.Value).Returns(options);
        return mock;
    }
}