using MassTransit;

namespace Justwish.Users.Contracts;

[EntityName("user-deleted-event")]
[MessageUrn("user-deleted-event")]
public sealed record UserDeletedEvent(Guid UserId, string Username, string Email);