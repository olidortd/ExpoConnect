using System.ComponentModel.DataAnnotations;

namespace ExpoConnect.Domain.Auth;

public class UserCredential
{
    // 1:1 with users (FK = user_id)
    [Key]
    public string UserId { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
}
