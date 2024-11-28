using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Justwish.Users.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Justwish.Users.Application;

public sealed class JwtEncoder : IJwtEncoder
{
    private readonly JwtOptions _options;

    public JwtEncoder(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }
    
    public JwtToken CreateToken(IEnumerable<Claim> userClaims, TimeSpan expirationTime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        Claim[] tokenClaims =
        [
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64),
        ];
        var claims = userClaims.Concat(tokenClaims);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: now.Add(expirationTime),
            signingCredentials: credentials
        );
        return new JwtToken(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public IEnumerable<Claim> DecodeToken(JwtToken token)
    {
        var handler = new JwtSecurityTokenHandler();

        var jwtToken = handler.ReadJwtToken(token.Token);

        return jwtToken.Claims;
    }

    public void ValidateToken(JwtToken token)
    {
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.Audience,
        };
        var claims = handler.ValidateToken(token.Token, validationParameters, out var validatedToken);
    }
}