namespace TaskFlow.Infrastructure.Settings;
public class JwtSettings
{
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string ValidAudience { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public bool RequireExpirationTime { get; set; }
    public string SecretKey { get; set; } = null!;
    public int AccessTokenLifetimeHours { get; set; } = 8;
    public int RememberMeLifetimeDays { get; set; } = 30;
}
