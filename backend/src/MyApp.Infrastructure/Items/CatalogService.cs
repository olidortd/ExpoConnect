// Infrastructure/Items/CatalogService.cs
using ExpoConnect.Domain.Expo;
using ExpoConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpoConnect.Infrastructure.Items;

public interface ICatalogService
{
    Task<CatalogItem> CreateAsync(CatalogItem item, CancellationToken ct);
    Task<CatalogItem?> GetAsync(Guid itemId, CancellationToken ct);
    Task<List<CatalogItem>> ListByStandAsync(string standId, CancellationToken ct);
    Task<CatalogItem?> UpdateAsync(Guid itemId, Action<CatalogItem> mutate, CancellationToken ct);
    Task<bool> DeleteAsync(Guid itemId, CancellationToken ct);
}

public class CatalogService : ICatalogService
{
    private readonly AppDbContext _db;
    public CatalogService(AppDbContext db) => _db = db;

    public async Task<CatalogItem> CreateAsync(CatalogItem item, CancellationToken ct)
    {
        _db.CatalogItems.Add(item);
        await _db.SaveChangesAsync(ct);
        return item;
    }

    public Task<CatalogItem?> GetAsync(Guid itemId, CancellationToken ct) =>
        _db.CatalogItems.FirstOrDefaultAsync(x => x.ItemId == itemId, ct);

    public Task<List<CatalogItem>> ListByStandAsync(string standId, CancellationToken ct) =>
        _db.CatalogItems.Where(x => x.StandId == standId).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<CatalogItem?> UpdateAsync(Guid itemId, Action<CatalogItem> mutate, CancellationToken ct)
    {
        var item = await _db.CatalogItems.FirstOrDefaultAsync(x => x.ItemId == itemId, ct);
        if (item is null) return null;
        mutate(item);
        await _db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<bool> DeleteAsync(Guid itemId, CancellationToken ct)
    {
        var item = await _db.CatalogItems.FirstOrDefaultAsync(x => x.ItemId == itemId, ct);
        if (item is null) return false;
        _db.CatalogItems.Remove(item);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
