using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record SendEmailVerificationCommand(string Email) : ICommand<Result>;