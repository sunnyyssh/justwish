using System.Net;
using System.Net.Mail;
using FastEndpoints;
using Justwish.Users.Domain;
using Justwish.Users.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Justwish.Users.FunctionalTests;

public sealed class UpdateUserProfileTests : EndpointTestBase
{
    [Fact]
    public async Task Not_Authorized()
    {
        // Arrange
        var request = new UpdateUserProfileEndpoint.UpdateRequest("Alex", "Gitto", null, Gender.Male, null);
        await ClearAuthorizationDefaultHeaderAsync();

        // Act
        var response = await Client.PUTAsync<UpdateUserProfileEndpoint, UpdateUserProfileEndpoint.UpdateRequest>(request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Updates_Valid_Authorized_Request()
    {
        // Arrange
        var request = new UpdateUserProfileEndpoint.UpdateRequest("Alex", "Gitto", null, Gender.Male, null);
        await SetAuthorizationDefaultHeaderAsync(TestData.User1);

        // Act
        var response = await Client.PUTAsync<UpdateUserProfileEndpoint,
            UpdateUserProfileEndpoint.UpdateRequest, Results<Ok, BadRequest<string>>>(request);

        // Assert
        Assert.True(response.Response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Updates_All_Null_Data_Authorized()
    {
        // Arrange
        var request = new UpdateUserProfileEndpoint.UpdateRequest(null, null, null, null, null);
        await SetAuthorizationDefaultHeaderAsync(TestData.User1);

        // Act
        var response = await Client.PUTAsync<UpdateUserProfileEndpoint,
            UpdateUserProfileEndpoint.UpdateRequest, Results<Ok, BadRequest<string>>>(request);

        // Assert
        Assert.True(response.Response.IsSuccessStatusCode);
    }

    [Theory]
    [MemberData(nameof(InvalidData))]
    public async Task Bad_Request_Invalid_Data_Authorized(
        string? firstName, string? lastName, DateOnly? dateOfBirth, Gender? gender, List<string>? socialLinks)
    {
        // Arrange
        var request = new UpdateUserProfileEndpoint.UpdateRequest(firstName, lastName, dateOfBirth, gender, socialLinks);
        await SetAuthorizationDefaultHeaderAsync(TestData.User1);

        // Act
        var response = await Client.PUTAsync<UpdateUserProfileEndpoint,
            UpdateUserProfileEndpoint.UpdateRequest, Results<Ok, BadRequest<string>>>(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.Response.StatusCode);
    }

    public static IEnumerable<object?[]> InvalidData =>
        [
            ["Odd spaces in first name", null, null, null, null],
            ["Invalid&^%Chars", null, null, null, null],
            [null, "Invalid&^%Chars", null, null, null],
            [null, "TooooooooooooooooooLoooooooooooooooooooongItIsMoreThan50Characters", null, null, null],
            [null, null, DateOnly.MaxValue, null, null],
            [null, null, null, (Gender)45, null],
            [null, null, null, null, Enumerable.Range(1, 1000).Select(i => "https://media.example.com").ToList()],
            [null, null, null, null, new List<string> { "smtp://not-http-link.com" }],
        ];
}
