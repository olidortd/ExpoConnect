// Domain/Expo/Visit.cs
using ExpoConnect.Domain;

namespace ExpoConnect.Domain.Expo;

public class Visit
{
    // PK: visit.visit_id (uuid)
    public Guid VisitId { get; set; } = Guid.NewGuid();
    public string VisitorId { get; set; } = default!; // FK -> users.user_id
    public string StandId { get; set; } = default!;    // FK -> stand.qr_code
    public string? Notes { get; set; }
    public int? Rating { get; set; } // 1-5
    public List<VisitSticker> Stickers { get; set; } = new();
    public bool IsFavorite { get; set; }
    public bool FollowUp { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // nav (optional)
    public Users.User Visitor { get; set; } = default!;
    public Stand Stand { get; set; } = default!;
}
