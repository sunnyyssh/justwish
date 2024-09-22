namespace Justwish.Users.WebApi.ApiKeyAuth;

public interface IApiKeyValidator
{
    ValueTask<bool> IsValidAsync(string apiKey);
}