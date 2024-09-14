using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record UserDto(Guid Id, string Email, string Username)
{
    public static UserDto FromDomain(User user) => new(user.Id, user.Email, user.Username);
}