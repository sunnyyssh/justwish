namespace Justwish.Users.Contracts;

public sealed record SendEmailVerificationCodeRequest(string Email, int Code);