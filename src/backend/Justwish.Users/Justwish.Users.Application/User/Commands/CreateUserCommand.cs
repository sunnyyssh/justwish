using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record CreateUserCommand(string Username, string Email, string Password) : ICommand<Result<UserDto>>;