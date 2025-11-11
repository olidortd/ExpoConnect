namespace ExpoConnect.Contracts.Items;

public record CatalogItemResponse(
    Guid ItemId,
    Guid CatalogId,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    string? ImageUrl,
    string[]? Features);

public record CreateCatalogItemRequest(
    Guid CatalogId,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    string? ImageUrl,
    string[]? Features);
