using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExpoConnect.Api.Auth;

public static class JwtConfig
{
    public static TokenValidationParameters Build(IConfiguration cfg)
    {
        var issuer = cfg["JWT:Issuer"] ?? throw new InvalidOperationException("JWT:Issuer missing");
        var audience = cfg["JWT:Audience"] ?? throw new InvalidOperationException("JWT:Audience missing");
        var keyStr = cfg["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));

        return new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = key,

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    }
}
