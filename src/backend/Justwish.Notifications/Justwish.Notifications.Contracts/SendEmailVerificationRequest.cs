namespace Justwish.Notifications.Contracts;

public sealed record SendEmailVerificationRequest(string Email, int Code);