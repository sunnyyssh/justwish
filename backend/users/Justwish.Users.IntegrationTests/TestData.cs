using System;
using Justwish.Users.Application;
using Justwish.Users.Domain;
using Justwish.Users.Infrastructure;

namespace Justwish.Users.IntegrationTests;

public static class TestData
{
    private static readonly DefaultPasswordHasher PasswordHasher = new();

    public const string User1Password = "bob_password";

    public const string User2Password = "bib_password";

    public static readonly User User1WithSharedPhoto1 = new("bob", "bob@test.com", PasswordHasher.Hash(User1Password))
    {
        FirstName = "Bob",
        LastName = "Bobito",
        DateOfBirth = new DateOnly(1990, 1, 1),
        Gender = Gender.Male,
        SocialLinks = [ "t.me/bob" ]
    };

    public static readonly ProfilePhoto SharedPhoto1 = new() 
    {
        Data = [1, 2, 3, 69],
        ContentType = "test/test",
        SharedPhotoAlias = "test",
    };

    public static readonly ProfilePhoto Photo2 = new()
    {
        Data = [1, 2, 3, 69],
        ContentType = "test/test"
    };

    public static readonly User User2WithOwnPhoto2 = new("bib", "bib@test.com", PasswordHasher.Hash(User2Password))
    {
        FirstName = "Bib",
        LastName = "Bibito",
        DateOfBirth = new DateOnly(1999, 2, 1),
        Gender = Gender.Female,
        SocialLinks = [ "t.me/bib" ]
    };

    public static void SeedData(ApplicationDbContext context) 
    {
        context.ProfilePhotos.Add(SharedPhoto1);
        context.ProfilePhotos.Add(Photo2);

        User1WithSharedPhoto1.ProfilePhotoId = SharedPhoto1.Id;
        User2WithOwnPhoto2.ProfilePhotoId = Photo2.Id;

        context.Users.Add(User1WithSharedPhoto1);
        context.Users.Add(User2WithOwnPhoto2);
        
        context.SaveChanges();
    }
}
