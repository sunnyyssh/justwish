using System;
using Justwish.Users.Infrastructure;

namespace Justwish.Users.IntegrationTests;

public sealed class EfProfilePhotoRepositoryTests : DatabaseTestBase
{
    [Fact]
    public async Task Returns_Random_Shared_Photo_Id()
    {
        // Arrange
        var repository = new EfProfilePhotoRepository(Context);

        // Act
        var result = await repository.GetRandomSharedPhotoIdAsync();

        bool isShared = await Context.ProfilePhotos.FindAsync(result.Value) is { IsShared: true };

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(isShared);
        Assert.NotEqual(Guid.Empty, result.Value);
    }
}
