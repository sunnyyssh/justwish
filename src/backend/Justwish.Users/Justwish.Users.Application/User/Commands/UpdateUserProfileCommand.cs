using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record UpdateUserProfileCommand(
    Guid UserId, 
    string? FirstName, 
    string? LastName, 
    DateOnly? DateOfBirth,
    Gender? Gender,
    List<string>? SocialLinks) 
        : ICommand<Result>;
