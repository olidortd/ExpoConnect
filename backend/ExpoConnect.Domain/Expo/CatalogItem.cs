// Domain/Expo/CatalogItem.cs
namespace ExpoConnect.Domain.Expo;

public class CatalogItem
{
    // PK: catalog_item.item_id (uuid)
    public Guid ItemId { get; set; } = Guid.NewGuid();
    public string StandId { get; set; } = default!; // FK -> stand.qr_code
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Category { get; set; }
    public string? Price { get; set; }
    public string? ImageUrl { get; set; }
    public string[]? Features { get; set; }

    // nav
    public Stand Stand { get; set; } = default!;
}
