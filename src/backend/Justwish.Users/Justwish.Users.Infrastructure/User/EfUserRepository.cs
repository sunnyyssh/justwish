using Ardalis.Result;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Justwish.Users.Infrastructure;

public sealed class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EfUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return user is null ? Result<User>.NotFound() : Result.Success(user);
    }

    public async Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync([userId], cancellationToken: cancellationToken);
        return user is null ? Result<User>.NotFound() : Result.Success(user);
    }

    public async Task<Result<User>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        return user is null ? Result<User>.NotFound() : Result.Success(user);
    }

    public async Task<Result> AddAsync(User user)
    {
        var loadedUser = await _dbContext.Users.FindAsync(user.Id);
        if (loadedUser is not null)
        {
            return Result.Error();
        }

        _dbContext.Add(user);
        int written = await _dbContext.SaveChangesAsync();

        return written > 0 ? Result.Success() : Result.Error();
    }

    public async Task<Result> UpdateAsync(User user)
    {
        var loadedUser = await _dbContext.Users.FindAsync(user.Id);
        if (loadedUser is null)
        {
            return Result.NotFound();
        }

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(User user)
    {
        var loadedUser = await _dbContext.Users.FindAsync(user.Id);
        if (loadedUser is null)
        {
            return Result.NotFound();
        }

        // User removal should cause its photo removal only if it is not shared.
        // Mention that if users have same photo that is not Shared, it will cause bugs.
        // Also cascade deletion seems to work faster, but account deletion is not a frequent operation.
        if (loadedUser.ProfilePhotoId is not null 
            && await _dbContext.ProfilePhotos.FindAsync(loadedUser.ProfilePhotoId) 
                is { IsShared: false } notSharedPhoto)
        {
            _dbContext.Remove(notSharedPhoto);
        }

        _dbContext.Remove(loadedUser);
        await _dbContext.SaveChangesAsync();
        
        return Result.Success();
    }
}