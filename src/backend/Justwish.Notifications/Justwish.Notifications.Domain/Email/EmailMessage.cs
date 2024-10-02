namespace Justwish.Notifications.Domain;

public sealed record EmailMessage
{
    public required EmailAddress To { get; init; }
}
