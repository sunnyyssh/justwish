using Ardalis.Result;

namespace Justwish.Users.Domain.Interfaces;

public interface IUserRepository : IUserReadRepository
{
    Task<Result> AddAsync(User user);
    
    Task<Result> UpdateAsync(User user);
    
    Task<Result> DeleteAsync(User user);
}