using System.Net;
using FastEndpoints;
using Justwish.Users.Contracts;
using Justwish.Users.Domain;
using Justwish.Users.WebApi;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Justwish.Users.FunctionalTests;

public sealed class CreateUserTests : IAsyncDisposable
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    private readonly ITestHarness _testHarness;

    public CreateUserTests()
    {
        _factory = new TestWebApplicationFactory();
        _client = _factory.CreateClient();
        _testHarness = _factory.Services.GetRequiredService<ITestHarness>();
    }

    [Theory]
    [InlineData("wrong@@wrongEmail", "valid_username", "valid_Password_6969")]
    [InlineData("validEmail@test.com", "o", "valid_Password_6969")]
    [InlineData("validEmail@test.com", "HasUpperCases", "valid_Password_6969")]
    [InlineData("validEmail@test.com", "valid_username1", "invalid&^!~Password_6969")]
    [InlineData("validEmail@test.com", "valid_username", "oo")]
    public async Task Doesnt_Create_NotValid_User(string email, string username, string password)
    {
        // Act
        var response = await _client.POSTAsync<CreateUserEndpoint,
            CreateUserEndpoint.RegistrationRequest>(
            new CreateUserEndpoint.RegistrationRequest(username, email, password));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Creates_Valid_Verified_User()
    {
        // Arrange
        const string email = "justwish@gmail.com";
        const string username = "justwish";
        const string password = "passwordSuperPuper_123";
        
        var verificationService = _factory.Services.GetRequiredService<IEmailVerificationService>();
        
        var code = await verificationService.IssueCodeAsync(email);
        await verificationService.VerifyEmailAsync(email, code);

        // Act
        var response = await _client.POSTAsync<CreateUserEndpoint,
            CreateUserEndpoint.RegistrationRequest, CreateUserEndpoint.RegisteredResponse>(
            new CreateUserEndpoint.RegistrationRequest(username, email, password));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.NotNull(response.Result.UserId);
        Assert.NotEqual(Guid.Empty, response.Result.UserId.Value);
    }

    [Fact]
    public async Task Publishes_UserCreatedEvent()
    {
        // Arrange
        const string email = "justwish@gmail.com";
        const string username = "justwish";
        const string password = "passwordSuperPuper_123";
        
        var verificationService = _factory.Services.GetRequiredService<IEmailVerificationService>();
        
        var code = await verificationService.IssueCodeAsync(email);
        await verificationService.VerifyEmailAsync(email, code);

        // Act
        var response = await _client.POSTAsync<CreateUserEndpoint,
            CreateUserEndpoint.RegistrationRequest, CreateUserEndpoint.RegisteredResponse>(
            new CreateUserEndpoint.RegistrationRequest(username, email, password));

        // Assert
        Assert.True(await _testHarness.Published.Any<UserCreatedEvent>());
    }

    [Fact]
    public async Task Doesnt_Create_Valid_NotVerified_User()
    {
        // Arrange
        const string email = "justwish@gmail.com";
        const string username = "justwish";
        const string password = "passwordSuperPuper_123";
        
        var verificationService = _factory.Services.GetRequiredService<IEmailVerificationService>();
        
        _ = await verificationService.IssueCodeAsync(email); // Issued but not verified.

        // Act
        var response = await _client.POSTAsync<CreateUserEndpoint,
            CreateUserEndpoint.RegistrationRequest>(
            new CreateUserEndpoint.RegistrationRequest(username, email, password));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
        _client.Dispose();
    }
}