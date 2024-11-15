﻿namespace Justwish.Users.Domain;

public interface IUserBusinessRulePredicates
{
    public Task<bool> IsUserEmailFree(string email);
    
    public Task<bool> IsUsernameFree(string username);
}