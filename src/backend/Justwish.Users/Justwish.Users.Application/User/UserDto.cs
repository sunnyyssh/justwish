using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed record UserDto(Guid Id, string Email, string Username)
{
    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public Guid? ProfilePhotoId { get; init; }

    public DateOnly? DateOfBirth { get; init; }

    public Gender? Gender { get; init; }

    public IReadOnlyList<string>? SocialLinks { get; init; }

    public static UserDto FromDomain(User user) => new(user.Id, user.Email, user.Username) 
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        ProfilePhotoId = user.ProfilePhotoId,
        DateOfBirth = user.DateOfBirth,
        Gender = user.Gender,
        SocialLinks = user.SocialLinks,
    };
}