using System.Net;
using FastEndpoints;
using Justwish.Users.Contracts;
using Justwish.Users.WebApi;
using MassTransit.Testing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Justwish.Users.FunctionalTests;

public sealed class SendEmailCodeTests : IAsyncDisposable
{
    private readonly HttpClient _client;
    
    private readonly ITestHarness _massTransitHarness;
    
    private readonly TestWebApplicationFactory _factory;
    
    public SendEmailCodeTests()
    {
        _factory = new TestWebApplicationFactory();
        _client = _factory.CreateClient();
        _massTransitHarness = _factory.Services.GetRequiredService<ITestHarness>();
    }

    [Fact]
    public async Task SendsCodeMqRequest_FreeEmail()
    {
        // Arrange
        const string email = "ThisEmailIsFree@FreeEmails.com";
        
        // Act
        var response = await _client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest, Ok>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.True(response.Response.IsSuccessStatusCode, "Response was not about success");
        Assert.True(await _massTransitHarness.Consumed.Any<SendEmailVerificationRequest>(),
            "SendEmailVerificationRequest was not sent");
    }

    [Fact]
    public async Task FailsValidation_WrongEmailFormat()
    {
        // Arrange
        const string email = "wrong_email@@wrongEmails.fake";
        
        // Act
        var response = await _client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest, Ok>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
        Assert.False(await _massTransitHarness.Consumed.Any<SendEmailVerificationRequest>(),
            "Why the f**k was SendEmailVerificationRequest sent");
    }

    [Fact]
    public async Task FailsValidation_EmailInUse()
    {
        // Arrange
        string email = SeedTestData.User1.Email;
        
        // Act
        var response = await _client.POSTAsync<SendEmailCodeEndpoint, SendEmailCodeEndpoint.EmailRequest, Ok>(
            new SendEmailCodeEndpoint.EmailRequest(email));
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
        Assert.False(await _massTransitHarness.Consumed.Any<SendEmailVerificationRequest>(),
            "Why the f**k was SendEmailVerificationRequest sent");
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
        _client.Dispose();
    }
}