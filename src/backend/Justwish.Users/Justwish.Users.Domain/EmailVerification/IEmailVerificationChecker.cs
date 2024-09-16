namespace Justwish.Users.Domain;

public interface IEmailVerificationChecker
{
    public Task<EmailVerificationStatus> GetStatusAsync(string email);
}