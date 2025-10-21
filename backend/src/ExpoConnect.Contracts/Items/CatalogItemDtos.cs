// Contracts/Items/CatalogItemDtos.cs
namespace ExpoConnect.Contracts.Items;

public record CatalogItemCreateRequest(string StandId, string Name, string Description, string? Category, string? Price, string? ImageUrl, string[]? Features);
public record CatalogItemUpdateRequest(string Name, string Description, string? Category, string? Price, string? ImageUrl, string[]? Features);
public record CatalogItemResponse(Guid ItemId, string StandId, string Name, string Description, string? Category, string? Price, string? ImageUrl, string[]? Features);
