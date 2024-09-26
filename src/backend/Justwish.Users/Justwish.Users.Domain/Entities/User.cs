using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Justwish.Users.Domain;

public sealed class User
{
    public User()
    { }

    [SetsRequiredMembers]
    public User(string username, string email, string passwordHash)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
    }

    public Guid Id { get; init; }

    public required string Username { get; init; }
    
    public required string Email { get; init; }
    
    public required string PasswordHash { get; set; }

    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }

    public Guid? ProfilePhotoId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public List<string>? SocialLinks { get; set; }
}