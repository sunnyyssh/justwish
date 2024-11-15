using Ardalis.Result;
using Justwish.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Justwish.Users.Infrastructure;

public class EfProfilePhotoRepository : IProfilePhotoRepository
{
    private readonly ApplicationDbContext _context;

    public EfProfilePhotoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> AddSharedAsync(ProfilePhoto profilePhoto)
    {
        if (!profilePhoto.IsShared)
        {
            return Result.Error("Profile photo is not shared");
        }
        var entry = _context.ProfilePhotos.Add(profilePhoto);
        await _context.SaveChangesAsync();
        return Result.Success(entry.Entity.Id);
    }

    public async Task<Result<Guid>> AddUserProfilePhotoAsync(ProfilePhoto profilePhoto)
    {
        var entry = _context.ProfilePhotos.Add(profilePhoto);
        await _context.SaveChangesAsync();
        return Result.Success(entry.Entity.Id);
    }

    public async Task<Result> DeleteAsync(ProfilePhoto profilePhoto)
    {
        var entry = _context.ProfilePhotos.Remove(profilePhoto);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<ProfilePhoto>> GetProfilePhotoByIdAsync(Guid id)
    {
        var photo = await _context.ProfilePhotos.FindAsync(id);
        return photo is null ? Result<ProfilePhoto>.NotFound() : Result.Success(photo);
    }

    public async Task<Result<Guid>> GetRandomSharedPhotoIdAsync(CancellationToken cancellationToken = default)
    {
        var photo = await _context.ProfilePhotos
            .Where(p => p.SharedPhotoAlias != null)
            .OrderBy(p => EF.Functions.Random())
            .Select(p => new { p.Id })
            .FirstOrDefaultAsync();

        return photo is not null
            ? Result.Success(photo.Id) 
            : Result<Guid>.NotFound();
    }

    public async Task<Result<ProfilePhoto>> GetSharedByAliasAsync(string alias)
    {
        var photo = await _context.ProfilePhotos
            .FirstOrDefaultAsync(p => p.SharedPhotoAlias == alias);
        return photo is null ? Result<ProfilePhoto>.NotFound() : Result.Success(photo);
    }

    public async Task<List<(Guid id, string alias)>> ListAllSharedAsync()
    {
        var sharedPhotos = await _context.ProfilePhotos
            .Where(p => p.SharedPhotoAlias != null)
            .Select(p => new { p.Id, p.SharedPhotoAlias })
            .ToListAsync();

        return sharedPhotos.Select(p => (p.Id, p.SharedPhotoAlias!)).ToList();
    }
}
