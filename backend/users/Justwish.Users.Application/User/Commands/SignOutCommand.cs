using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record SignOutCommand(JwtToken Token) : ICommand;