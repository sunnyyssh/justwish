using System.Net;
using FastEndpoints;
using Justwish.Users.WebApi;
using Microsoft.AspNetCore.Http;

namespace Justwish.Users.FunctionalTests;

public class UploadProfilePhotoTests : EndpointTestBase
{
    [Fact]
    public async Task Uploads_And_Returns_Id() 
    {
        // Arrange
        byte[] data = [69, 69, 69, 69, 69, 69];
        var stream = new MemoryStream(data);
        IFormFile file = new FormFile(stream, 0, data.Length, "testimg", "test.jpg") 
        {
            Headers = new HeaderDictionary(), 
            ContentType = "image/jpg" 
        };
        var request = new UploadProfilePhotoEndpoint.UploadRequest(file);

        await SetAuthorizationDefaultHeaderAsync(TestData.User3);        

        // Act
        var response = await Client.POSTAsync<UploadProfilePhotoEndpoint, 
            UploadProfilePhotoEndpoint.UploadRequest, 
            UploadProfilePhotoEndpoint.UploadResponse>(request, true);

        // Assert
        Assert.True(response.Response.IsSuccessStatusCode);
        Assert.NotEqual(Guid.Empty, response.Result.PhotoId);
    }

    [Fact]
    public async Task Returns_Unautorized_NoJwt()
    {
        // Arrange
        byte[] data = [42, 69, 42, 69, 42, 69];
        var stream = new MemoryStream(data);
        IFormFile file = new FormFile(stream, 0, data.Length, "testimg", "test.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpg"
        };
        var request = new UploadProfilePhotoEndpoint.UploadRequest(file);

        await ClearAuthorizationDefaultHeaderAsync();

        // Act
        var response = await Client.POSTAsync<UploadProfilePhotoEndpoint,
            UploadProfilePhotoEndpoint.UploadRequest,
            UploadProfilePhotoEndpoint.UploadResponse>(request, true);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
    }
}

