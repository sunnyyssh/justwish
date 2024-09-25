using System.Net;
using FastEndpoints;
using Justwish.Users.Contracts;
using Justwish.Users.WebApi;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.FunctionalTests;

public sealed class SendEmailCodeTests : EndpointTestBase
{
    [Fact]
    public async Task SendsCodeMqRequest_FreeEmail()
    {
        // Arrange
        const string email = "ThisEmailIsFree@FreeEmails.com";
        
        // Act
        var response = await Client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest, Ok>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.True(response.Response.IsSuccessStatusCode, "Response was not about success");
        Assert.True(await MassTransitTestHarness.Consumed.Any<SendEmailVerificationRequest>(),
            "SendEmailVerificationRequest was not sent");
    }

    [Fact]
    public async Task FailsValidation_WrongEmailFormat()
    {
        // Arrange
        const string email = "wrong_email@@wrongEmails.fake";
        
        // Act
        var response = await Client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest, Ok>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
        Assert.False(await MassTransitTestHarness.Consumed.Any<SendEmailVerificationRequest>(),
            "Why the f**k was SendEmailVerificationRequest sent");
    }

    [Fact]
    public async Task FailsValidation_EmailInUse()
    {
        // Arrange
        string email = TestData.User1.Email;
        
        // Act
        var response = await Client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest, Ok>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
        Assert.False(await MassTransitTestHarness.Consumed.Any<SendEmailVerificationRequest>(),
            "Why the f**k was SendEmailVerificationRequest sent");
    }
}