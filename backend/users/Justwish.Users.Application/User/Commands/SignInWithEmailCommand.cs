using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record SignInWithEmailCommand(string Email, string Password) : ICommand<Result<JwtTokenPair>>;