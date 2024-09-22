namespace Justwish.Users.WebApi.ApiKeyAuth;

public sealed class ConfigurationApiKeyValidator : IApiKeyValidator
{
    private readonly HashSet<string> _apiKeys;

    public ConfigurationApiKeyValidator(IConfiguration configuration)
    {
        _apiKeys = configuration.GetSection(ApiKeyConstants.ConfigurationSection)
            .AsEnumerable()
            .Where(entry => entry.Value is not null)
            .Select(entry => entry.Value!)
            .ToHashSet();
    }
    
    public ValueTask<bool> IsValidAsync(string apiKey)
    {
        return ValueTask.FromResult(_apiKeys.Contains(apiKey));
    }
}