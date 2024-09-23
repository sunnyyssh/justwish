using System.Net;
using FastEndpoints;
using Justwish.Users.WebApi;
using Justwish.Users.WebApi.ApiKeyAuth;

namespace Justwish.Users.FunctionalTests;

public sealed class SignInTests : EndpointTestBase
{
    [Fact]
    public async Task SignIn_With_ValidEmail_Password()
    {
        // Arrange
        string email = TestData.User1.Email;
        string password = TestData.User1Password;
        
        // Act
        var response =
            await Client.POSTAsync<SignInEndpoint, SignInEndpoint.SignInRequest, SignInEndpoint.SignInResponse>(
                new SignInEndpoint.SignInRequest(email, null, password));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
    }

    [Fact]
    public async Task SignIn_With_ValidUsername_Password()
    {
        // Arrange
        string username = TestData.User1.Username;
        string password = TestData.User1Password;
        
        // Act
        var response =
            await Client.POSTAsync<SignInEndpoint, SignInEndpoint.SignInRequest, SignInEndpoint.SignInResponse>(
                new SignInEndpoint.SignInRequest(null, username, password));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
    }

    [Fact]
    public async Task Doesnt_SignIn_With_Invalid_Password()
    {
        // Arrange
        string email = TestData.User1.Email;
        string password = "wrong_password";
        
        // Act
        var response =
            await Client.POSTAsync<SignInEndpoint, SignInEndpoint.SignInRequest, SignInEndpoint.SignInResponse>(
                new SignInEndpoint.SignInRequest(email, null, password));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    [Fact]
    public async Task Unauthorized_With_No_ApiKey()
    {
        // Arrange
        const string email = "test@test.com";        
        const string password = "test_password";

        Client.DefaultRequestHeaders.Remove(ApiKeyConstants.HeaderName);
        
        // Act
        var response =
            await Client.POSTAsync<SignInEndpoint, SignInEndpoint.SignInRequest, SignInEndpoint.SignInResponse>(
                new SignInEndpoint.SignInRequest(email, null, password));
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
    }
}