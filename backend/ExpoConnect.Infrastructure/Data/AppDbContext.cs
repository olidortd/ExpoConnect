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

        b.HasPostgresEnum<VisitSticker>();
        b.HasPostgresExtension("uuid-ossp");

        b.Entity<Visit>(e =>
        {
            e.Property(x => x.Stickers)
             .HasColumnType("text[]")
             .Metadata.SetValueComparer(stickersComparer);
        });

        b.Entity<CatalogItem>().Ignore("stand_number");

        b.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.UserId);
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Email).HasColumnName("email").IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.DisplayName).HasColumnName("display_name");
            e.Property(x => x.Phone).HasColumnName("phone");
            e.Property(x => x.Company).HasColumnName("company");

            e.Property(x => x.Role).HasColumnName("role").HasConversion<string>().HasColumnType("text");

            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
            e.Property(x => x.RefreshTokenHash).HasColumnName("refresh_token_hash");
            e.Property(x => x.RefreshTokenExpiresAtUtc).HasColumnName("refresh_token_expires_at_utc");
        });

        b.ApplyConfiguration(new Mappings.UserMap());
        b.ApplyConfiguration(new Mappings.UserCredentialMap());
        b.ApplyConfiguration(new Mappings.RefreshTokenMap());
        b.ApplyConfiguration(new Mappings.StandMap());
        b.ApplyConfiguration(new Mappings.VisitMap());
        b.ApplyConfiguration(new Mappings.CatalogMap());
        b.ApplyConfiguration(new Mappings.CatalogItemMap());
    }
}
