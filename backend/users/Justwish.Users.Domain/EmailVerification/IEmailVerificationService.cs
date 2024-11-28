namespace Justwish.Users.Domain;

public interface IEmailVerificationService : IEmailVerificationIssuer, IEmailVerifier, IEmailVerificationChecker;