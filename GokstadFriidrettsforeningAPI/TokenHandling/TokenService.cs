using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
namespace GokstadFriidrettsforeningAPI.TokenHandling;

public class TokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    public string GenerateToken(int memberId, string email)
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, email),
            new("MemberId", memberId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        SymmetricSecurityKey key = new (Encoding.UTF8.GetBytes(jwtOptions.Value.Key!));
        SigningCredentials credentials = new (key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new (
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}