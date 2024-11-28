using Justwish.Users.Application;

namespace Justwish.Users.UnitTests;

public class DefaultPasswordHasherTests
{
    [Fact]
    public void Verifies_Password()
    {
        // Arrange
        var hasher = new DefaultPasswordHasher();
        const string password = "My_Ultimate_Password_12345";

        // Act
        string hash = hasher.Hash(password);
        bool verified = hasher.Verify(password, hash);

        // Assert
        Assert.True(verified);
    }

    [Fact]
    public void Doesnt_Verify_Another_Password()
    {
        // Arrange
        var hasher = new DefaultPasswordHasher();
        const string password = "My_Ultimate_Password_12345";
        const string fakePassword = "Worst_Password_Ever_69";

        // Act
        string hash = hasher.Hash(password);
        bool verified = hasher.Verify(fakePassword, hash);

        // Assert
        Assert.False(verified);
    }
}