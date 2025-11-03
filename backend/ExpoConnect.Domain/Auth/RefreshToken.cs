// Domain/Auth/RefreshToken.cs
using System.ComponentModel.DataAnnotations;

namespace ExpoConnect.Domain.Auth;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }

    // FK -> users.user_id (string)
    public string UserId { get; set; } = default!;
    public Users.User User { get; set; } = default!;

    public bool IsActive => RevokedAtUtc is null && DateTime.UtcNow < ExpiresAtUtc;
}
