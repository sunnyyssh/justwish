namespace Justwish.Users.Domain;

public interface IEmailVerifier
{
    public Task<bool> VerifyEmailAsync(string email, int code);
}