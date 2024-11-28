using Ardalis.Result;
using Justwish.Users.Application;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Justwish.Users.UnitTests;

public sealed class SendEmailVerificationHandlerTests
{
    [Fact]
    public async Task Sends_Generated_Code()
    {
        // Arrange
        const string email = "test@test.com";
        const int code = 6969;

        var mockLogger = new Mock<ILogger<SendEmailVerificationHandler>>();
        var mockPublishEndpoint = MockPublishEndpoint();
        var mockVerificationIssuer = MockVerificationIssuer(code);

        var handler = new SendEmailVerificationHandler(mockPublishEndpoint.Object,
            mockVerificationIssuer.Object, mockLogger.Object);

        // Act
        await handler.Handle(new SendEmailVerificationCommand(email), default);

        // Assert
        VerifyPublishedOnce(mockPublishEndpoint);
    }

    private static void VerifyPublishedOnce(Mock<IPublishEndpoint> mockPublishEndpoint)
    {
        mockPublishEndpoint.Verify(c =>
            c.Publish(It.IsAny<SendEmailVerificationRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static Mock<IEmailVerificationIssuer> MockVerificationIssuer(int returningCode)
    {
        var mockIssuer = new Mock<IEmailVerificationIssuer>();
        mockIssuer.Setup(x => x.IssueCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(Result.Success(returningCode));
        return mockIssuer;
    }

    private static Mock<IPublishEndpoint> MockPublishEndpoint()
    {
        var mockPublishEndpoint = new Mock<IPublishEndpoint>();
        mockPublishEndpoint.Setup(x => x.Publish(It.IsAny<SendEmailVerificationRequest>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return mockPublishEndpoint;
    }
}