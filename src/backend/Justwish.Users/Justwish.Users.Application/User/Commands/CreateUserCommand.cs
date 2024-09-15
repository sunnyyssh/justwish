using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public record CreateUserCommand(string Username, string Email, string Password) : ICommand<Result<UserDto>>;