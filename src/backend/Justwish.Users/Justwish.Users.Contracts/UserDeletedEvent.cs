namespace Justwish.Users.Contracts;

public sealed record UserDeletedEvent(Guid UserId, string Username, string Email);