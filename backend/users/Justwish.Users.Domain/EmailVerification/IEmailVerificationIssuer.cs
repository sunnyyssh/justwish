using Ardalis.Result;

namespace Justwish.Users.Domain;

public interface IEmailVerificationIssuer
{
    public Task<Result<int>> IssueCodeAsync(string email);
}