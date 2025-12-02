namespace QR.Models;

public class CatalogResponse : ContentPage
{
    public Guid CatalogId { get; set; }
    public string StandId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // מהשרת מגיע items: [ ... ]
    public List<CatalogItemResponse> Items { get; set; } = new();

}