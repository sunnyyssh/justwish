using Justwish.Users.Application;
using Justwish.Users.Domain;
using Justwish.Users.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Justwish.Users.FunctionalTests;

public static class SeedTestData
{
    private static readonly IPasswordHasher PasswordHasher = new DefaultPasswordHasher();

    public static readonly User User1 = new()
    {
        Email = "bob@example.com", Username = "bob", PasswordHash = PasswordHasher.Hash("BobSecret_123")
    };

    public static readonly User User2 = new()
    {
        Email = "mary@example.com", Username = "mary", PasswordHash = PasswordHasher.Hash("MarySecret_123"),
    };

    public static readonly User User3 = new()
    {
        Email = "alex@example.com", Username = "alex", PasswordHash = PasswordHasher.Hash("AlexSecret_123"),
    };
    
    public static void PopulateTestData(ApplicationDbContext context)
    {
        context.Users.AddRange(User1, User2, User3);
        context.SaveChanges();
    }
}