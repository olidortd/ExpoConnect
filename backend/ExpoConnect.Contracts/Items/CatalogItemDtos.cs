using System.ComponentModel.DataAnnotations;

namespace ExpoConnect.Contracts.Items;

public record CatalogItemResponse(
    string ItemId,
    string StandId,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    string? ImageUrl,
    string? Features
);

public class CreateCatalogItemRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = default!;

    [StringLength(1000)]
    public string? Description { get; init; }

    [Required]
    [StringLength(50)]
    public string Category { get; init; } = default!;

    [Range(0, 1_000_000)]
    public decimal Price { get; init; }

    [Url]
    public string? ImageUrl { get; init; }

    [StringLength(2000)]
    public string? Features { get; init; }
}

public class UpdateCatalogItemRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = default!;

    [StringLength(1000)]
    public string? Description { get; init; }

    [Required]
    [StringLength(50)]
    public string Category { get; init; } = default!;

    [Range(0, 1_000_000)]
    public decimal Price { get; init; }

    [Url]
    public string? ImageUrl { get; init; }

    [StringLength(2000)]
    public string? Features { get; init; }
}
