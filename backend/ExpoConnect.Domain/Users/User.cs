using ExpoConnect.Domain.Auth;

namespace ExpoConnect.Domain.Users;

public class User
{
    // PK: users.user_id (string)
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = default!;
    public string? DisplayName { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public UserRole Role { get; set; } = UserRole.visitor;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string PasswordHash { get; set; } = default!;
    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }

    // nav
    public UserCredential? Credential { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
