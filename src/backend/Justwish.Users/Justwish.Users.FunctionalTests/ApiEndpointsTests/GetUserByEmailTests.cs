using System;
using System.Net;
using FastEndpoints;
using Justwish.Users.WebApi;

namespace Justwish.Users.FunctionalTests;

public class GetUserByEmailTests : EndpointTestBase
{
    [Fact]
    public async Task Gets_User_Exisiting_Email()
    {
        // Arrange
        string email = TestData.User1.Email;

        // Act
        var response =
            await Client.GETAsync<GetUserByEmailEndpoint, GetUserByEmailEndpoint.GetUserRequest, GetUserResponse>(
                new GetUserByEmailEndpoint.GetUserRequest(email));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal(email, response.Result.Email);
    }

    [Fact]
    public async Task Returns_NotFound()
    {
        // Arrange
        string email = "notExisting@email.com";

        // Act
        var response =
            await Client.GETAsync<GetUserByEmailEndpoint, GetUserByEmailEndpoint.GetUserRequest, GetUserResponse>(
                new GetUserByEmailEndpoint.GetUserRequest(email));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.Response.StatusCode);
    }

    [Fact]
    public async Task Returns_BadRequest_InvalidEmail()
    {
        // Arrange
        string email = "$invalid@@email.com";

        // Act
        var response =
            await Client.GETAsync<GetUserByEmailEndpoint, GetUserByEmailEndpoint.GetUserRequest, GetUserResponse>(
                new GetUserByEmailEndpoint.GetUserRequest(email));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }
}
