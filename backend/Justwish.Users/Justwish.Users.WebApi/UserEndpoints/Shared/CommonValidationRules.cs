using System;
using FluentValidation;

namespace Justwish.Users.WebApi;

public static class CommonValidationRules
{
    public static IRuleBuilderOptions<T, string> Username<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(32)
            .Matches(@"^[a-z0-9_]+$")
            .WithMessage("Username must contain only lowercase alphanumeric characters and underscores");
    }

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(32)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Password must contain only alphanumeric characters and underscores");
    }
}
