namespace Justwish.Users.Contracts;

public sealed record SendEmailVerificationRequest(string Email, int Code);