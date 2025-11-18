using ExpoConnect.Contracts.Catalogs;
using ExpoConnect.Contracts.Items;
using ExpoConnect.Domain.Expo;

namespace ExpoConnect.Application.Interfaces;

public interface ICatalogService
{
    Task<List<CatalogResponse>> ListAsync(CancellationToken ct);
    Task<CatalogResponse?> GetAsync(Guid catalogId, CancellationToken ct);
    Task<CatalogResponse> CreateAsync(CreateCatalogRequest req, CancellationToken ct);
    Task<CatalogItemResponse> AddItemAsync(Guid catalogId, CreateCatalogItemRequest req, CancellationToken ct);
    Task<CatalogItem?> UpdateItemAsync(Guid itemId, UpdateCatalogItemRequest request, CancellationToken ct);
    Task<bool> DeleteItemAsync(Guid itemId, CancellationToken ct);
}
