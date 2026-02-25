using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskNext.Api.Models; // Namespace updated to Next

namespace TaskNext.Api.Services; // Namespace updated to Next

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; }
}

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) CreateToken(User user);
}

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwt;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwt = jwtOptions.Value;
    }

public (string token, DateTime expiresAtUtc) CreateToken(User user)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // Claims update karein:
    var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new("name", user.Name),
        // YE LINE ADD KAREIN: Iske bina TasksController user ko nahi pehchanega
        new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString())
    };

    var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiresMinutes);

    var token = new JwtSecurityToken(
        issuer: _jwt.Issuer,
        audience: _jwt.Audience,
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    return (new JwtSecurityTokenHandler().WriteToken(token), expires);
}
}