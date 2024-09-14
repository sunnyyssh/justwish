using Justwish.Users.Domain.Interfaces;

namespace Justwish.Users.Domain;

public sealed class DefaultUserBusinessRulePredicates : IUserBusinessRulePredicates
{
    private readonly IUserRepository _repository;

    public DefaultUserBusinessRulePredicates(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<bool> IsUserEmailFree(string email)
    {
        return await _repository.GetUserByEmailAsync(email) is { IsSuccess: false};
    }

    public async Task<bool> IsUsernameFree(string username)
    {
        return await _repository.GetUserByUsernameAsync(username) is { IsSuccess: false };
    }
}