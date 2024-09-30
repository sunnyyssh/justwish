using System.Diagnostics.CodeAnalysis;

namespace Justwish.Users.Domain;

public sealed class ProfilePhoto
{
    public Guid Id { get; init; }

    public required byte[] Data { get; init; }

    public required string ContentType { get; init; }

    public string? SharedPhotoAlias { get; init; }

    [MemberNotNullWhen(true, nameof(SharedPhotoAlias))]
    public bool IsShared => !string.IsNullOrEmpty(SharedPhotoAlias);
}
