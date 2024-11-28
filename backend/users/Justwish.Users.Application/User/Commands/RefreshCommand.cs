using System.Windows.Input;
using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record RefreshCommand(JwtToken RefreshToken) : ICommand<Result<JwtTokenPair>>;