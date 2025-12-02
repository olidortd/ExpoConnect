using System.ComponentModel.DataAnnotations;

namespace ExpoConnect.Contracts.Catalogs;

public record CatalogResponse(
    Guid CatalogId,
    string StandId,
    string Name,
    string Description,
    DateTime CreatedAt,
    List<Items.CatalogItemResponse> Items);

public record CreateCatalogRequest(
    [Required]
    string StandId,
    [Required]
    string Name,
    [Required]
    string Description);
