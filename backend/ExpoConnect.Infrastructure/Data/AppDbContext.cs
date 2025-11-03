// Data/AppDbContext.cs
using ExpoConnect.Domain;
using ExpoConnect.Domain.Users;
using ExpoConnect.Domain.Expo;
//using ExpoConnect.Domain.Whitelist;
using ExpoConnect.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection.Emit;


namespace ExpoConnect.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserCredential> UserCredentials => Set<UserCredential>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Stand> Stands => Set<Stand>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<Visit> Visits => Set<Visit>();
    //public DbSet<WhitelistedEmail> WhitelistedEmails => Set<WhitelistedEmail>();

    protected override void OnModelCreating(ModelBuilder b)
    {

        var stickersConverter = new ValueConverter<List<VisitSticker>, string[]>(
            v => v.Select(x => x.ToString()).ToArray(),
            v => (v ?? Array.Empty<string>()).Select(s => Enum.Parse<VisitSticker>(s, true)).ToList()
        );

        var stickersComparer = new ValueComparer<List<VisitSticker>>(
            (a, b) => a.SequenceEqual(b),
            v => v.Aggregate(0, (h, x) => HashCode.Combine(h, x.GetHashCode())),
            v => v.ToList()
        );

        b.Entity<Visit>(e =>
        {
            e.Property(x => x.Stickers)
             .HasConversion(stickersConverter)
             .HasColumnType("text[]")
             .Metadata.SetValueComparer(stickersComparer);
        });

        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
