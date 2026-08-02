using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.ServicesInterfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Settings;

namespace TaskFlow.Infrastructure.Services;
public class JwtService(IOptions<JwtSettings> options, ICacheService cacheService) : IJwtService
{
    private readonly JwtSettings _jwtSettings = options.Value;
    private readonly ICacheService _cacheService = cacheService;

    public string GenerateToken(User user, bool staySignedIn = false)
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

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
    public async Task<T> ValidateToken<T>(string token)
    {
        var cleanToken = token.Trim();
        var cacheKey = $"jwt:blacklist:{cleanToken}";
        var isBlacklisted = await _cacheService.GetAsync<bool>(cacheKey);

        if (isBlacklisted) throw new SecurityTokenException("Token has been revoked.");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        try
        {
            tokenHandler.ValidateToken(cleanToken, new TokenValidationParameters
            {
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuer = _jwtSettings.ValidIssuer,

                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidAudience = _jwtSettings.ValidAudience,

                ValidateLifetime = _jwtSettings.ValidateLifetime,

                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return JsonSerializer.Deserialize<T>(
                jwtToken.Payload.SerializeToJson(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? throw new SecurityTokenException();
        }
        catch(Exception ex)
        {
            throw new SecurityTokenException("Invalid token", ex);
        }

    }


}
