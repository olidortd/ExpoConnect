using ExpoConnect.Contracts.Catalogs;
using ExpoConnect.Contracts.Items;

namespace ExpoConnect.Application.Interfaces;

public interface ICatalogService
{
    Task<List<CatalogResponse>> ListAsync(CancellationToken ct);
    Task<CatalogResponse?> GetAsync(Guid catalogId, CancellationToken ct);
    Task<CatalogResponse> CreateAsync(CreateCatalogRequest req, CancellationToken ct);
    Task<CatalogItemResponse> AddItemAsync(CreateCatalogItemRequest req, CancellationToken ct);
}
