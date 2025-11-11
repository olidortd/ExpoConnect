// Domain/Expo/Catalog.cs
namespace ExpoConnect.Domain.Expo;

public class Catalog
{
    public Guid CatalogId { get; set; }
    public string StandId { get; set; } = null!;
    public Stand Stand { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CatalogItem> Items { get; set; } = new List<CatalogItem>();
}
