using System.Text;
using System.Threading.RateLimiting;
using Justwish.Users.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Justwish.Users.WebApi;

public class JwtBearerConfigureOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _options;

    public JwtBearerConfigureOptions(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme) 
        {
            Configure(options);
        }
    }

    public void Configure(JwtBearerOptions options)
    {
        // This turns off mapping of claim types. 
        // Without this line "sub" jwt claim is mapped to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier".
        // It seems implicit at now.
        options.MapInboundClaims = false; 
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateLifetime = true,
        };
    }
}