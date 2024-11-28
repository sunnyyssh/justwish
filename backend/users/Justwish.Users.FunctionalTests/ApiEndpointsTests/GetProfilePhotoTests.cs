using System.Net;
using FastEndpoints;
using Justwish.Users.Domain;
using Justwish.Users.WebApi;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Justwish.Users.FunctionalTests;

public class GetProfilePhotoTests : EndpointTestBase
{
    [Fact]
    public async Task Not_Found_When_Not_Existing_Id()
    {
        // Arrange
        var request = new GetProfilePhotoEndpoint.PhotoRequest(Guid.NewGuid());

        // Act
        var response = await Client.GETAsync<GetProfilePhotoEndpoint, 
            GetProfilePhotoEndpoint.PhotoRequest, FileContentHttpResult>(request);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.Response.StatusCode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Gets_Photo_Existing_Id(bool sharedPhoto)
    {
        // Arrange
        var photo = sharedPhoto ? TestData.SharedPhoto1 : TestData.Photo2;
        var request = new GetProfilePhotoEndpoint.PhotoRequest(photo.Id);

        // Act
        var response = await Client.GETAsync<GetProfilePhotoEndpoint, 
            GetProfilePhotoEndpoint.PhotoRequest>(request);
        var content = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(contentType);
        Assert.Equal(photo.ContentType, contentType.MediaType);
        Assert.Equal(photo.Data, content);
    }
}
