using System.Net;
using FastEndpoints;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using Justwish.Users.WebApi;
using MassTransit.Testing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Justwish.Users.FunctionalTests;

public sealed class VerifyEmailTests : IAsyncDisposable
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public VerifyEmailTests()
    {
        _factory = new TestWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task VerifiesEmail_WithIssuedCode()
    {
        // Arrange
        const string email = "test@test.com";
        var codeIssuer = _factory.Services.GetRequiredService<IEmailVerificationIssuer>();
        int code = await codeIssuer.IssueCodeAsync(email);
        
        // Act
        var response =
            await _client
                .POSTAsync<VerifyEmailCodeEndpoint, VerifyEmailCodeEndpoint.EmailCodeRequest,
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
            await _client
                .POSTAsync<VerifyEmailCodeEndpoint, VerifyEmailCodeEndpoint.EmailCodeRequest,
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
        var response = await _client.POSTAsync<VerifyEmailCodeEndpoint, VerifyEmailCodeEndpoint.EmailCodeRequest,
            VerifyEmailCodeEndpoint.VerificationStatusResponse>(
            new VerifyEmailCodeEndpoint.EmailCodeRequest(email, code));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    [Fact]
    public async Task FailsValidation_EmailInUse()
    {
        // Arrange
        string email = SeedTestData.User1.Email;
        
        // Act
        var response = await _client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest,
            VerifyEmailCodeEndpoint.VerificationStatusResponse>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
        _client.Dispose();
    }
}