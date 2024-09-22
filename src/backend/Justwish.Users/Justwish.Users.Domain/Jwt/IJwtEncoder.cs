using System.Security.Claims;

namespace Justwish.Users.Domain;

public interface IJwtEncoder
{
    public JwtToken CreateToken(IEnumerable<Claim> userClaims, TimeSpan expirationTime);

    public IEnumerable<Claim> DecodeToken(JwtToken token);

    public void ValidateToken(JwtToken token);
}