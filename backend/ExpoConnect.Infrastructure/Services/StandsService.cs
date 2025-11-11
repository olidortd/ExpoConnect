using ExpoConnect.Application.Interfaces;
using ExpoConnect.Contracts.Stands;
using ExpoConnect.Domain.Expo;
using ExpoConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpoConnect.Infrastructure.Services;

public class StandsService(AppDbContext db) : IStandsService
{
    private readonly AppDbContext _db = db;

    public async Task<StandResponse> CreateAsync(CreateStandRequest req, CancellationToken ct)
    {
        var exists = await _db.Set<Stand>().AnyAsync(s => s.QrCode == req.QrCode, ct);
        if (exists) throw new InvalidOperationException($"Stand '{req.QrCode}' already exists.");

        var s = new Stand
        {
            QrCode = req.QrCode,
            StandNumber = req.StandNumber,
            CompanyName = req.CompanyName,
            Description = req.Description,
            Industry = req.Industry,
            ContactEmail = req.ContactEmail,
            ContactPhone = req.ContactPhone,
            Website = req.Website,
            LogoUrl = req.LogoUrl,
            BannerUrl = req.BannerUrl,
            ExhibitorId = req.ExhibitorId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Add(s);
        await _db.SaveChangesAsync(ct);
        return new StandResponse(s.QrCode, s.StandNumber, s.CompanyName);
    }

    public Task<bool> ExistsAsync(string standId, CancellationToken ct)
        => _db.Set<Stand>().AnyAsync(s => s.QrCode == standId, ct);
}
