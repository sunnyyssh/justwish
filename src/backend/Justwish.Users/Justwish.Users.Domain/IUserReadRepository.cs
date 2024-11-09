using Ardalis.Result;

namespace Justwish.Users.Domain;

public interface IUserReadRepository
{
    Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<Result<User>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
}