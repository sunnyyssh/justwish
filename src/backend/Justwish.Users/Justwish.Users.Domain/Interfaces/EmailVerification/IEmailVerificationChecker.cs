namespace Justwish.Users.Domain;

public interface IEmailVerificationChecker
{
    public bool VerifyEmail(string email, int code);
}