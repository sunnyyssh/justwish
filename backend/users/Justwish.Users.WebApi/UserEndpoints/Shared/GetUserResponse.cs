using Justwish.Users.Application;

namespace Justwish.Users.WebApi;

public record GetUserResponse(Guid Id, string Email, string Username)
{
    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public Guid? ProfilePhotoId { get; init; }

    public DateOnly? DateOfBirth { get; init; }

    public string? Gender { get; init; }

    public IReadOnlyList<string>? SocialLinks { get; init; }

    public static GetUserResponse FromUserDto(UserDto userDto)
        => new(userDto.Id, userDto.Email, userDto.Username)
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            ProfilePhotoId = userDto.ProfilePhotoId,
            DateOfBirth = userDto.DateOfBirth,
            Gender = userDto.Gender.ToString(),
            SocialLinks = userDto.SocialLinks,
        };
};