namespace Justwish.Users.Application;

public sealed class EmailVerificationOptions
{
    public int CodeLifetimeSeconds { get; set; } = 60;

    public int CodeLength { get; set; } = 4;
}