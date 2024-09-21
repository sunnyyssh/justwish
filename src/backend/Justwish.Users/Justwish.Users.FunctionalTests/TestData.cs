﻿using Justwish.Users.Application;
using Justwish.Users.Domain;
using Justwish.Users.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Justwish.Users.FunctionalTests;

public static class TestData
{
    private static readonly IPasswordHasher PasswordHasher = new DefaultPasswordHasher();

    public const string User1Password = "BobSecret_123";

    public const string User2Password = "MarySecret_123";

    public const string User3Password = "AlexSecret_123";

    public static readonly User User1 = new()
    {
        Email = "bob@example.com", Username = "bob", PasswordHash = PasswordHasher.Hash(User1Password)
    };

    public static readonly User User2 = new()
    {
        Email = "mary@example.com", Username = "mary", PasswordHash = PasswordHasher.Hash(User2Password),
    };

    public static readonly User User3 = new()
    {
        Email = "alex@example.com", Username = "alex", PasswordHash = PasswordHasher.Hash(User3Password),
    };
    
    public static void PopulateTestData(ApplicationDbContext context)
    {
        context.Users.AddRange(User1, User2, User3);
        context.SaveChanges();
    }
}