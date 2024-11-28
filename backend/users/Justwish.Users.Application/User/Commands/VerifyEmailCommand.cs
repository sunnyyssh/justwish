using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record VerifyEmailCommand(string Email, int Code) : ICommand<bool>;