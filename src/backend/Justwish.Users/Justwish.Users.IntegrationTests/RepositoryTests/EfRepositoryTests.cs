using System;
using Justwish.Users.Infrastructure;

namespace Justwish.Users.IntegrationTests;

public class EfRepositoryTests : DatabaseTestBase
{
    [Fact]
    public async Task UserRemoval_Doesnt_Remove_Shared_Photo() 
    {
        // Arrange
        var user = TestData.User1WithSharedPhoto1;
        var photo = TestData.SharedPhoto1;
        var repository = new EfUserRepository(Context);

        // Act
        await repository.DeleteAsync(user);
        bool isPhotoRemoved = await Context.ProfilePhotos.FindAsync(photo.Id) is null;

        // Assert
        Assert.False(isPhotoRemoved);
    }

    [Fact]
    public async Task UserRemoval_Removes_Photo_Not_Shared()
    {
        // Arrange
        var user = TestData.User2WithOwnPhoto2;
        var photo = TestData.Photo2;
        var repository = new EfUserRepository(Context);

        // Act
        await repository.DeleteAsync(user);
        bool isPhotoRemoved = await Context.ProfilePhotos.FindAsync(photo.Id) is null;

        // Assert
        Assert.True(isPhotoRemoved);
    }
}
