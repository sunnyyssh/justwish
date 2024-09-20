namespace Justwish.Users.Contracts;

public record UserCreatedEvent(Guid UserId, string Name, string Email);