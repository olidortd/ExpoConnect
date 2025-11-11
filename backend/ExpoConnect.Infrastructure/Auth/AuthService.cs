using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ExpoConnect.Application.Interfaces;
using ExpoConnect.Contracts.Auth;
using ExpoConnect.Domain.Users;
using ExpoConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExpoConnect.Infrastructure.Auth;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly int _accessMinutes;
    private readonly int _refreshDays;

    public AuthService(AppDbContext db, IConfiguration cfg)
    {
        _db = db;

        var jwt = cfg.GetSection("Jwt");
        _issuer = jwt["Issuer"] ?? throw new InvalidOperationException("Missing Jwt:Issuer");
        _audience = jwt["Audience"] ?? throw new InvalidOperationException("Missing Jwt:Audience");
        var key = jwt["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        _accessMinutes = int.TryParse(jwt["AccessTokenMinutes"], out var m) ? m : 60;
        _refreshDays = int.TryParse(jwt["RefreshTokenDays"], out var d) ? d : 7;
    }

    // === Public API ===

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
    {
        var email = NormalizeEmail(req.Email);
        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Email = email,
            DisplayName = req.DisplayName,
            Role = Domain.UserRole.visitor,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 11)
        };

        // issue refresh + access
        var refresh = GenerateRefreshToken();
        user.RefreshTokenHash = HashRefreshToken(refresh);
        user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_refreshDays);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var (access, expiresAt) = GenerateAccessToken(user);
        return new AuthResponse(access, refresh, expiresAt);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct = default)
    {
        var email = NormalizeEmail(req.Email);
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email, ct);
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!ok) throw new UnauthorizedAccessException("Invalid credentials.");

        // rotate refresh
        var refresh = GenerateRefreshToken();
        user.RefreshTokenHash = HashRefreshToken(refresh);
        user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_refreshDays);
        await _db.SaveChangesAsync(ct);

        var (access, expiresAt) = GenerateAccessToken(user);
        return new AuthResponse(access, refresh, expiresAt);
    }

public async Task<MeResponse> GetMeAsync(string userId, CancellationToken ct = default)
{
    var me = await _db.Users
        .Where(u => u.UserId == userId)
        .Select(u => new MeResponse(
            u.UserId,
            u.Email,
            u.DisplayName,
            u.Role.ToString()
        ))
        .SingleOrDefaultAsync(ct);

    if (me is null)
        throw new KeyNotFoundException("User not found.");

    return me;
}

public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var hash = HashRefreshToken(refreshToken);
        var user = await _db.Users.SingleOrDefaultAsync(u => u.RefreshTokenHash == hash, ct);

        if (user is null || !user.IsActive ||
            user.RefreshTokenExpiresAtUtc is null ||
            user.RefreshTokenExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        // rotate refresh
        var newRefresh = GenerateRefreshToken();
        user.RefreshTokenHash = HashRefreshToken(newRefresh);
        user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_refreshDays);
        await _db.SaveChangesAsync(ct);

        var (access, expiresAt) = GenerateAccessToken(user);
        return new AuthResponse(access, newRefresh, expiresAt);
    }

    // === Helpers ===

    private (string token, DateTime expiresAtUtc) GenerateAccessToken(User user)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_accessMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, ToUnix(now).ToString(), ClaimValueTypes.Integer64)
        };

        var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, expires);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var s = Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return s;
    }

    private static string HashRefreshToken(string refreshToken)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }

    private static long ToUnix(DateTime dt) => (long)(dt - DateTime.UnixEpoch).TotalSeconds;
    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
