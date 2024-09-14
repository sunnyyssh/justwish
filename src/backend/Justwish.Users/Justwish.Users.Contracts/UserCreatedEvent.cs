namespace Justwish.Users.Contracts;

public record UserCreatedEvent(Guid Id, string Name, string Email);