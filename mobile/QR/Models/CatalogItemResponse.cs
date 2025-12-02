namespace QR.Models;

public class CatalogItemResponse : ContentPage
{



    public Guid ItemId { get; set; }
    public Guid StandId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }   // "imageUrl" од-JSON
    public string[] Features { get; set; }

}