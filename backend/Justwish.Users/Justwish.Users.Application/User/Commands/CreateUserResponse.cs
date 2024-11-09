using System.Diagnostics.CodeAnalysis;

namespace Justwish.Users.Application;

public sealed class CreateUserResponse
{
    [MemberNotNullWhen(false, nameof(User))]
    public bool IsEmailNotVerified { get; private init; }
    
    public UserDto? User { get; private init; }

    public static CreateUserResponse Success(UserDto user) => new CreateUserResponse { User = user };
    
    public static CreateUserResponse EmailNotVerified() => new CreateUserResponse { IsEmailNotVerified = true };
}