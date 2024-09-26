using System;

namespace Justwish.Users.Domain.Entities;

public sealed class UserProfilePhoto
{
    public Guid Id { get; init; }

    public required byte[] Data { get; init; }

    public required string ContentType { get; init; }
}
