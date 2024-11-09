namespace Justwish.Users.Application;

public class ProfilePhotoDto
{
    public required Guid Id { get; init; }

    public required string ContentType { get; init; }

    public required byte[] Data { get; init; }
}
