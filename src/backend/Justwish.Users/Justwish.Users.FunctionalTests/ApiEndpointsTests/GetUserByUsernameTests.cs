using System;
using System.Net;
using FastEndpoints;
using Justwish.Users.WebApi;

namespace Justwish.Users.FunctionalTests;

public class GetUserByUsernameTests : EndpointTestBase
{
    [Fact]
    public async Task Gets_User_Existing_Username()
    {
        // Arrange
        string username = TestData.User1.Username;

        // Act
        var response =
            await Client.GETAsync<GetUserByUsernameEndpoint, GetUserByUsernameEndpoint.GetUserRequest, GetUserResponse>(
                new GetUserByUsernameEndpoint.GetUserRequest(username));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal(username, response.Result.Username);
    }

    [Fact]
    public async Task Returns_NotFound()
    {
        // Arrange
        string username = "not_existing_username";

        // Act
        var response =
            await Client.GETAsync<GetUserByUsernameEndpoint, GetUserByUsernameEndpoint.GetUserRequest, GetUserResponse>(
                new GetUserByUsernameEndpoint.GetUserRequest(username));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.Response.StatusCode);
    }

    [Fact]
    public async Task Returns_BadRequest_InvalidUsername()
    {
        // Arrange
        string username = "not$Existing_username";

        // Act
        var response =
            await Client.GETAsync<GetUserByUsernameEndpoint, GetUserByUsernameEndpoint.GetUserRequest, GetUserResponse>(
                new GetUserByUsernameEndpoint.GetUserRequest(username));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }
}
