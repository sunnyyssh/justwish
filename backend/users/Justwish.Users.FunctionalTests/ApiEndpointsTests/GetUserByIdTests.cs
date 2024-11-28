using System.Net;
using FastEndpoints;
using Justwish.Users.WebApi;

namespace Justwish.Users.FunctionalTests;

public class GetUserByIdTests : EndpointTestBase
{
    [Fact]
    public async Task Gets_User_Exisiting_Id()
    {
        // Arrange
        Guid id = TestData.User1.Id;

        // Act
        var response =
            await Client.GETAsync<GetUserByIdEndpoint, GetUserByIdEndpoint.GetUserRequest, GetUserResponse>(
                new GetUserByIdEndpoint.GetUserRequest(id));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
        Assert.Equal(id, response.Result.Id);
    }

    [Fact]
    public async Task Returns_NotFound()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        var response =
            await Client.GETAsync<GetUserByIdEndpoint, GetUserByIdEndpoint.GetUserRequest, GetUserResponse>(
                new GetUserByIdEndpoint.GetUserRequest(id));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.Response.StatusCode);
    }
}
