using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.UnitTests;

public sealed class MockUserRepository : IUserRepository
{
    private readonly List<User> _users;
    
    public MockUserRepository(IEnumerable<User> users)
    {
        _users = users.ToList();
    }
    
    public Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var result = _users.SingleOrDefault(u => u.Email == email) is { } user
            ? Result.Success(user)
            : Result.NotFound();
        return Task.FromResult(result);
    }

    public Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = _users.SingleOrDefault(u => u.Id == userId) is { } user
            ? Result.Success(user)
            : Result.NotFound();
        return Task.FromResult(result);
    }

    public Task<Result<User>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var result = _users.SingleOrDefault(u => u.Username == username) is { } user
            ? Result.Success(user)
            : Result.NotFound();
        return Task.FromResult(result);
    }

    public Task<Result> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Add(user);
        return Task.FromResult(new Result());
    }

    public Task<Result> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.RemoveAll(u => u.Id == user.Id);
        _users.Add(user);
        return Task.FromResult(new Result());
    }

    public Task<Result> DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Remove(user);
        return Task.FromResult(new Result());
    }
}