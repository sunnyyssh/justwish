using System.ComponentModel.DataAnnotations;

namespace Justwish.Users.Domain;

public sealed class User
{
    public Guid Id { get; set; }

    public required string Username { get; init; }
    
    public required string Email { get; init; }
    
    public required string PasswordHash { get; init; }
}