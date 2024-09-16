using Ardalis.Result;
using Castle.Core.Logging;
using Justwish.Users.Application;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using MassTransit;
using MassTransit.Clients;
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
        var mockRequestClient = MockRequestClient();
        var mockVerificationIssuer = MockVerificationIssuer(code);

        var handler = new SendEmailVerificationHandler(mockRequestClient.Object, mockVerificationIssuer.Object,
            mockLogger.Object);
        
        // Act
        await handler.Handle(new SendEmailVerificationCommand(email), default);
        
        // Assert
        VerifySentOnce(mockRequestClient);
    }

    private static void VerifySentOnce(Mock<IRequestClient<SendEmailVerificationRequest>> mockRequestClient)
    {
        mockRequestClient.Verify(c =>
            c.GetResponse<SendEmailVerificationResponse>(
                It.IsAny<SendEmailVerificationRequest>(),
                It.IsAny<CancellationToken>(), 
                It.IsAny<RequestTimeout>()),
            Times.Once);
    }
    
    private static Mock<IEmailVerificationIssuer> MockVerificationIssuer(int returningCode)
    {
        var mockIssuer = new Mock<IEmailVerificationIssuer>();
        mockIssuer.Setup(x => x.IssueCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(Result.Success(returningCode));
        return mockIssuer;
    }

    private static Mock<IRequestClient<SendEmailVerificationRequest>> MockRequestClient()
    {
        var mockConsumeContext = new Mock<ConsumeContext<SendEmailVerificationResponse>>();
        mockConsumeContext.SetupGet(c => c.Message)
            .Returns(new SendEmailVerificationResponse(true));
        
        var mockRequestClient = new Mock<IRequestClient<SendEmailVerificationRequest>>();
        
        mockRequestClient.Setup(c =>
                c.GetResponse<SendEmailVerificationResponse>(
                    It.IsAny<SendEmailVerificationRequest>(),
                    It.IsAny<CancellationToken>(), 
                    It.IsAny<RequestTimeout>()))
            .ReturnsAsync(new MessageResponse<SendEmailVerificationResponse>(mockConsumeContext.Object));

        return mockRequestClient;
    }
}