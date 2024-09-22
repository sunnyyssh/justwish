using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Justwish.Users.WebApi.ApiKeyAuth;

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeySchemeOptions>
{
    private readonly IApiKeyValidator _validator;

    public ApiKeyAuthenticationHandler(IApiKeyValidator validator
        , IOptionsMonitor<ApiKeySchemeOptions> optionsMonitor, ILoggerFactory logger, UrlEncoder urlEncoder) 
        : base(optionsMonitor, logger, urlEncoder)
    {
        _validator = validator;
    }


    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var isAnonymousEndpoint =
            Context.GetEndpoint()?.Metadata.OfType<AllowAnonymousAttribute>().Any() is null or true;
        
        if (!isAnonymousEndpoint && !await HasValidApiKeyAsync())
        {
            return AuthenticateResult.Fail($"Invalid API key. Maybe it is missing in {ApiKeyConstants.HeaderName}");
        }

        var claim = new Claim(ApiKeyConstants.ClaimType, ApiKeyConstants.DefaultClaimValue);
        var identity = new ClaimsIdentity(claims: [claim], Scheme.Name);
        var principal = new GenericPrincipal(identity, roles: null);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private async ValueTask<bool> HasValidApiKeyAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyConstants.HeaderName, out var header))
            return false;
        
        foreach (var key in header)
        {
            if (key is not null && await _validator.IsValidAsync(key))
            {
                return true;
            }
        }

        return false;
    }
}