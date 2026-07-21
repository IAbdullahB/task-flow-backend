using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.ServicesInterfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Settings;

namespace TaskFlow.Infrastructure.Servicese;
public class JwtService(IOptions<JwtSettings> options) : IJwtService
{
    private readonly JwtSettings _jwtSettings = options.Value;
    public Task<string> GenerateToken(User user, bool staySignedIn = false)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var expiration = staySignedIn 
            ? DateTime.UtcNow.AddDays(_jwtSettings.RememberMeLifetimeDays) 
            : DateTime.UtcNow.AddHours(_jwtSettings.AccessTokenLifetimeHours);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));

    }

    public T ValidateToken<T>(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.ValidIssuer,

                ValidateAudience = true,
                ValidAudience = _jwtSettings.ValidAudience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return JsonSerializer.Deserialize<T>(
                jwtToken.Payload.SerializeToJson(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? throw new Exception();
        }
        catch
        {
            throw new SecurityTokenException("Invalid token");
        }

    }
}
