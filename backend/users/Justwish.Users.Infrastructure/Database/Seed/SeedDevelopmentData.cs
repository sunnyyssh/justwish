using Justwish.Users.Application;
using Justwish.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Justwish.Users.Infrastructure;

public static class SeedDevelopmentData
{
    private static readonly IPasswordHasher PasswordHasher = new DefaultPasswordHasher();
    
    public static async Task SeedAsync(DbContext dbContext, bool storeOpPerformed, CancellationToken cancellationToken = default)
    {
        var users = dbContext.Set<User>();
        if (await users.AnyAsync(cancellationToken: cancellationToken))
        {
            return;
        }

        users.AddRange(
            new User
            {
                Email = "bob@example.com", Username = "bob", PasswordHash = PasswordHasher.Hash("BobSecret_123")
            },
            new User
            {
                Email = "mary@example.com", Username = "mary", PasswordHash = PasswordHasher.Hash("MarySecret_123"),
            },
            new User
            {
                Email = "alex@example.com", Username = "alex", PasswordHash = PasswordHasher.Hash("AlexSecret_123"),
            });
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return;
    }
}