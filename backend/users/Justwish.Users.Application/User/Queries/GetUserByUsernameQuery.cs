using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record GetUserByUsernameQuery(string Username) : IQuery<Result<UserDto>>;
