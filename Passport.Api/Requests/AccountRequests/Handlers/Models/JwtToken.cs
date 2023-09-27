using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace Passport.Passport.Api.Requests.AccountRequests.Handlers;

public class JwtToken
{
    public readonly JwtSecurityToken Value;

    public JwtToken(Guid Id, string SecretKey, string Issuer, int ExpiresHours)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var identity = new ClaimsIdentity(new[] { new Claim("_id", Id.ToString())});

        var handler = new JwtSecurityTokenHandler();

        Value = handler.CreateJwtSecurityToken(
            subject: identity,
            issuer: Issuer, 
            expires: DateTime.UtcNow.AddHours(ExpiresHours),
            issuedAt: DateTime.UtcNow,
            signingCredentials: credentials);
    }
}
