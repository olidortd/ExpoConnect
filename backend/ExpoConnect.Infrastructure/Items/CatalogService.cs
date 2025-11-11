using ExpoConnect.Application.Interfaces;
using ExpoConnect.Contracts.Catalogs;
using ExpoConnect.Contracts.Items;
using ExpoConnect.Domain.Expo;
using ExpoConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpoConnect.Infrastructure.Items;

public class CatalogService : ICatalogService
{
    private readonly AppDbContext _db;
    public CatalogService(AppDbContext db) => _db = db;

    public async Task<List<CatalogResponse>> ListAsync(CancellationToken ct)
    {
        var catalogs = await _db.Set<Catalog>()
            .Include(c => c.Items)
            .ToListAsync(ct);

        return catalogs.Select(MapToResponse).ToList();
    }

    public async Task<CatalogResponse?> GetAsync(Guid catalogId, CancellationToken ct)
    {
        var catalog = await _db.Set<Catalog>()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CatalogId == catalogId, ct);

        return catalog is null ? null : MapToResponse(catalog);
    }

    public async Task<CatalogResponse> CreateAsync(CreateCatalogRequest req, CancellationToken ct)
    {
        var standExists = await _db.Set<Stand>().AnyAsync(s => s.QrCode == req.StandId, ct);
        if (!standExists)
            throw new KeyNotFoundException($"Stand '{req.StandId}' not found");

        var catalog = new Catalog
        {
            CatalogId = Guid.NewGuid(),
            StandId = req.StandId,
            Name = req.Name,
            Description = req.Description,
            CreatedAt = DateTime.UtcNow
        };

        _db.Add(catalog);
        await _db.SaveChangesAsync(ct);

        return MapToResponse(catalog);
    }

    public async Task<CatalogItemResponse> AddItemAsync(CreateCatalogItemRequest req, CancellationToken ct)
    {
        var exists = await _db.Set<Catalog>()
            .AnyAsync(c => c.CatalogId == req.CatalogId, ct);
        if (!exists)
            throw new KeyNotFoundException($"Catalog {req.CatalogId} not found");

        var item = new CatalogItem
        {
            ItemId = Guid.NewGuid(),
            CatalogId = req.CatalogId,
            Name = req.Name,
            Description = req.Description,
            Category = req.Category,
            Price = req.Price,
            ImageUrl = req.ImageUrl,
            Features = req.Features
        };

        _db.Add(item);
        await _db.SaveChangesAsync(ct);

        return new CatalogItemResponse(
            item.ItemId, item.CatalogId, item.Name, item.Description,
            ConvertCategory(item.Category), item.Price, item.ImageUrl, item.Features);
    }

    private static CatalogResponse MapToResponse(Catalog c) =>
        new(
            c.CatalogId,
            c.StandId,
            c.Name,
            c.Description,
            c.CreatedAt,
            c.Items.Select(i => new CatalogItemResponse(
                i.ItemId,
                i.CatalogId,
                i.Name,
                i.Description,
                ConvertCategory(i.Category),
                i.Price,
                i.ImageUrl,
                i.Features)).ToList()
        );

    private static string ConvertCategory(object category)
        => category switch
        {
            string s => s,
            Enum e => e.ToString(),
            _ => category?.ToString() ?? string.Empty
        };
}
