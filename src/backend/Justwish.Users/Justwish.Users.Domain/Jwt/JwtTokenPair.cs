namespace Justwish.Users.Domain;

public sealed record JwtTokenPair(JwtToken AccessToken, JwtToken RefreshToken);