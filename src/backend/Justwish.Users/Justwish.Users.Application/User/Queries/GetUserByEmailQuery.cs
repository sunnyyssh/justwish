using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record GetUserByEmailQuery(string Email) : IQuery<Result<UserDto>>;