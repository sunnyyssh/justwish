using MassTransit;

namespace Justwish.Users.Contracts;

[EntityName("user-created-event")]
[MessageUrn("user-created-event")]
public record UserCreatedEvent(Guid UserId, string Name, string Email);