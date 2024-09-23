using System.Net;
using FastEndpoints;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using Justwish.Users.WebApi;
using Justwish.Users.WebApi.ApiKeyAuth;
using MassTransit.Testing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Justwish.Users.FunctionalTests;

public sealed class VerifyEmailTests : EndpointTestBase
{
    [Fact]
    public async Task VerifiesEmail_WithIssuedCode()
    {
        // Arrange
        const string email = "test@test.com";
        var codeIssuer = Factory.Services.GetRequiredService<IEmailVerificationIssuer>();
        int code = await codeIssuer.IssueCodeAsync(email);
        
        // Act
        var response =
            await Client.POSTAsync<VerifyEmailCodeEndpoint, VerifyEmailCodeEndpoint.EmailCodeRequest,
                    VerifyEmailCodeEndpoint.VerificationStatusResponse>(
                    new VerifyEmailCodeEndpoint.EmailCodeRequest(email, code));
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.True(response.Result.Verified);
    }

    [Fact]
    public async Task DoesntVerifyEmail_WithNotIssuedCode()
    {
        // Arrange
        const string email = "test@test.com";
        const int code = 6969; // Not issued.
        
        // Act
        var response =
            await Client.POSTAsync<VerifyEmailCodeEndpoint, VerifyEmailCodeEndpoint.EmailCodeRequest,
                    VerifyEmailCodeEndpoint.VerificationStatusResponse>(
                    new VerifyEmailCodeEndpoint.EmailCodeRequest(email, code));
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.False(response.Result.Verified);
    }
    
    [Fact]
    public async Task FailsValidation_WrongEmailFormat()
    {
        // Arrange
        const string email = "wrong_email@@wrongEmails.fake";
        const int code = 6969; // Not issued.
        
        // Act
        var response = await Client.POSTAsync<VerifyEmailCodeEndpoint, VerifyEmailCodeEndpoint.EmailCodeRequest,
            VerifyEmailCodeEndpoint.VerificationStatusResponse>(
            new VerifyEmailCodeEndpoint.EmailCodeRequest(email, code));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    [Fact]
    public async Task FailsValidation_EmailInUse()
    {
        // Arrange
        string email = TestData.User1.Email;
        
        // Act
        var response = await Client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest,
            VerifyEmailCodeEndpoint.VerificationStatusResponse>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    [Fact]
    public async Task Unauthorized_With_No_ApiKey()
    {
        // Arrange
        const string email = "test@test.com";
        Client.DefaultRequestHeaders.Remove(ApiKeyConstants.HeaderName);
        
        // Act
        var response = await Client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest,
            VerifyEmailCodeEndpoint.VerificationStatusResponse>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
    }
}