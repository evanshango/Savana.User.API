using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Savana.User.API.Entities;
using Savana.User.API.Interfaces;

namespace Savana.User.API.Services;

public class TokenService : ITokenService {
    private readonly SymmetricSecurityKey _key;
    private readonly string _issuer;
    private readonly string _expiresIn;

    public TokenService(SymmetricSecurityKey key, string issuer, string expiresIn) {
        _key = key;
        _issuer = issuer;
        _expiresIn = expiresIn;
    }

    public string GenerateToken(UserEntity user) {
        var expiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_expiresIn));
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new List<Claim> {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Actor, user.Id)
        };

        var groups = user.UserGroups.Select(ug => ug.Group).ToList();
        if (groups is { Count: > 0 }) {
            var roles = new List<string?>();
            foreach (var group in groups) {
                roles.AddRange(
                    group?.RoleGroups.Select(rg => rg.Role?.Name).ToList() ?? new List<string?>()
                );
            }

            claims.AddRange(
                new HashSet<string>(roles!).Select(rl => new Claim(ClaimTypes.Role, rl))
            );
        }

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials = credentials,
            Issuer = _issuer,
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}