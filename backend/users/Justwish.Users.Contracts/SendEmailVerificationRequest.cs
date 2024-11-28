using MassTransit;

namespace Justwish.Users.Contracts;

[EntityName("send_email_verification")]
[MessageUrn("send_email_verification")]
public sealed record SendEmailVerificationRequest(string Email, int Code);