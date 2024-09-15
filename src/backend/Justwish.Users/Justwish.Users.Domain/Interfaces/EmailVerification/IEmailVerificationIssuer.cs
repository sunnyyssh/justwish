namespace Justwish.Users.Domain;

public interface IEmailVerificationIssuer
{
    public int IssueEmailVerificationCode(string email);
}