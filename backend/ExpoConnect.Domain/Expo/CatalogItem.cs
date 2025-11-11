namespace ExpoConnect.Domain.Expo;

public class CatalogItem
{
    public Guid ItemId { get; set; }
    public Guid CatalogId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string[]? Features { get; set; }

    public Catalog Catalog { get; set; } = null!;
}
