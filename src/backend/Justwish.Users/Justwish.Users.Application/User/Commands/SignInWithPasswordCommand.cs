using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record SignInWithUsernameCommand(string Username, string Password) : ICommand<Result<JwtTokenPair>>;