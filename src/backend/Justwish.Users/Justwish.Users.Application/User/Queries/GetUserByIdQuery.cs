using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<Result<UserDto>>;