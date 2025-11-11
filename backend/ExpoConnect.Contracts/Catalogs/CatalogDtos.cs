namespace ExpoConnect.Contracts.Catalogs;

public record CatalogResponse(
    Guid CatalogId,
    string StandId,
    string Name,
    string Description,
    DateTime CreatedAt,
    List<Items.CatalogItemResponse> Items);

public record CreateCatalogRequest(
    string StandId,
    string Name,
    string Description);
