namespace ExpoConnect.Domain.Expo;

public class Stand
{
    // PK: stand.qr_code (string)
    public string QrCode { get; set; } = default!;
    public string StandNumber { get; set; } = default!;
    public string CompanyName { get; set; } = default!;
    public string? Description { get; set; }
    public string? Industry { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string ExhibitorId { get; set; } = default!; // FK -> users.user_id
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // nav
    public Users.User Exhibitor { get; set; } = default!;
    public List<Catalog> Catalog { get; set; } = new();
}
