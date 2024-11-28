using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record DeleteUserCommand(Guid UserId) : ICommand<Result>;