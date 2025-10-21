// Infrastructure/Auth/AuthService.cs
using ExpoConnect.Domain.Auth;
using ExpoConnect.Domain.Users;
using ExpoConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpoConnect.Infrastructure.Auth;

public interface IAuthService
{
    Task<User> RegisterAsync(string email, string password, string? displayName, CancellationToken ct);
    Task<(User user, string access, string refresh)> LoginAsync(string email, string password, CancellationToken ct);
    Task<(string access, string newRefresh)> RefreshAsync(string refreshToken, CancellationToken ct);
    Task LogoutAsync(string refreshToken, CancellationToken ct);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly JwtOptions _opts;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(AppDbContext db, IJwtTokenService jwt, Microsoft.Extensions.Options.IOptions<JwtOptions> opts)
    { _db = db; _jwt = jwt; _opts = opts.Value; }

    public async Task<User> RegisterAsync(string email, string password, string? displayName, CancellationToken ct)
    {
        email = email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            throw new InvalidOperationException("Email already registered.");

        var user = new User { Email = email, DisplayName = displayName, IsActive = true };
        var cred = new UserCredential { UserId = user.UserId };
        cred.PasswordHash = _hasher.HashPassword(user, password);

        using var tx = await _db.Database.BeginTransactionAsync(ct);
        _db.Users.Add(user);
        _db.UserCredentials.Add(cred);
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return user;
    }

    public async Task<(User user, string access, string refresh)> LoginAsync(string email, string password, CancellationToken ct)
    {
        email = email.Trim().ToLowerInvariant();
        var user = await _db.Users.Include(u => u.Credential).FirstOrDefaultAsync(u => u.Email == email, ct)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive) throw new UnauthorizedAccessException("User inactive.");

        var vr = _hasher.VerifyHashedPassword(user, user.Credential!.PasswordHash, password);
        if (vr == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var (access, _) = _jwt.CreateAccessToken(user);

        var refresh = _jwt.CreateRefreshToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = refresh,
            UserId = user.UserId,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_opts.RefreshTokenDays)
        });
        await _db.SaveChangesAsync(ct);

        return (user, access, refresh);
    }

    public async Task<(string access, string newRefresh)> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var tokenRow = await _db.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == refreshToken, ct);
        if (tokenRow is null || !tokenRow.IsActive) throw new UnauthorizedAccessException("Invalid refresh token.");

        // rotate
        tokenRow.RevokedAtUtc = DateTime.UtcNow;
        var newRefresh = _jwt.CreateRefreshToken();
        tokenRow.ReplacedByToken = newRefresh;
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefresh,
            UserId = tokenRow.UserId,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_opts.RefreshTokenDays)
        });

        var (access, _) = _jwt.CreateAccessToken(tokenRow.User);
        await _db.SaveChangesAsync(ct);
        return (access, newRefresh);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct)
    {
        var tokenRow = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken, ct);
        if (tokenRow is null) return;
        tokenRow.RevokedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }
}
