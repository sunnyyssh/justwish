using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application.Commands;

public record CreateUserCommand(string Username, string Email, string Password) : ICommand<Result<UserDto>>;