namespace Justwish.Users.Application;

public sealed class EmailVerificationOptions
{
    public int CodeLength { get; set; } = 4;
    
    public int CodePersistenceSeconds { get; set; } = 60;

    public int VerifiedPersistenceSeconds { get; set; } = 300;
}