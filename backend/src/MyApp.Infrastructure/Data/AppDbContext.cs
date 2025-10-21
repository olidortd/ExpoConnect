// Infrastructure/Data/AppDbContext.cs
using ExpoConnect.Domain;
using ExpoConnect.Domain.Auth;
using ExpoConnect.Domain.Expo;
using ExpoConnect.Domain.Users;
//using ExpoConnect.Domain.Whitelist;
using Microsoft.EntityFrameworkCore;

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
        // Register Postgres enums
        b.HasPostgresEnum<UserRole>("user_role_enum");
        b.HasPostgresEnum<StandIndustry>("stand_industry_enum");
        b.HasPostgresEnum<VisitSticker>("visit_sticker_enum");

        // users
        b.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.UserId).HasName("pk_users");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Email).HasColumnName("email").IsRequired();
            e.Property(x => x.DisplayName).HasColumnName("display_name");
            e.Property(x => x.Phone).HasColumnName("phone");
            e.Property(x => x.Company).HasColumnName("company");
            e.Property(x => x.Role).HasColumnName("role").HasColumnType("user_role_enum").IsRequired();
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");

            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.Role);
            e.HasIndex(x => x.IsActive);

            e.HasOne(x => x.Credential)
             .WithOne()
             .HasForeignKey<UserCredential>(c => c.UserId)
             .HasConstraintName("fk_user_credentials_users")
             .OnDelete(DeleteBehavior.Cascade);
        });

        // user_credentials (aux table for auth)
        b.Entity<UserCredential>(e =>
        {
            e.ToTable("user_credentials");
            e.HasKey(x => x.UserId).HasName("pk_user_credentials");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
        });

        // auth_refresh_token (aux table for auth)
        b.Entity<RefreshToken>(e =>
        {
            e.ToTable("auth_refresh_token");
            e.HasKey(x => x.Id).HasName("pk_auth_refresh_token");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Token).HasColumnName("token").IsRequired();
            e.Property(x => x.ExpiresAtUtc).HasColumnName("expires_at_utc");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.Property(x => x.RevokedAtUtc).HasColumnName("revoked_at_utc");
            e.Property(x => x.ReplacedByToken).HasColumnName("replaced_by_token");

            e.Property(x => x.UserId).HasColumnName("user_id");
            e.HasOne(x => x.User)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(x => x.UserId)
             .HasConstraintName("fk_auth_refresh_token_users")
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.Token).IsUnique();
        });

        // stand
        b.Entity<Stand>(e =>
        {
            e.ToTable("stand");
            e.HasKey(x => x.QrCode).HasName("pk_stand");
            e.Property(x => x.QrCode).HasColumnName("qr_code");
            e.Property(x => x.StandNumber).HasColumnName("stand_number").IsRequired();
            e.Property(x => x.CompanyName).HasColumnName("company_name").IsRequired();
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.Industry).HasColumnName("industry").HasColumnType("stand_industry_enum");
            e.Property(x => x.ContactEmail).HasColumnName("contact_email");
            e.Property(x => x.ContactPhone).HasColumnName("contact_phone");
            e.Property(x => x.Website).HasColumnName("website");
            e.Property(x => x.LogoUrl).HasColumnName("logo_url");
            e.Property(x => x.BannerUrl).HasColumnName("banner_url");
            e.Property(x => x.ExhibitorId).HasColumnName("exhibitor_id").IsRequired();
            e.Property(x => x.CreatedAt).HasColumnName("created_at");

            e.HasOne(x => x.Exhibitor)
             .WithMany()
             .HasForeignKey(x => x.ExhibitorId)
             .HasConstraintName("fk_stand_users")
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.ExhibitorId);
            e.HasIndex(x => x.StandNumber);
        });

        // catalog_item  (our “Items”)
        b.Entity<CatalogItem>(e =>
        {
            e.ToTable("catalog_item");
            e.HasKey(x => x.ItemId).HasName("pk_catalog_item");
            e.Property(x => x.ItemId).HasColumnName("item_id");
            e.Property(x => x.StandId).HasColumnName("stand_id").IsRequired();
            e.Property(x => x.Name).HasColumnName("name").IsRequired();
            e.Property(x => x.Description).HasColumnName("description").IsRequired();
            e.Property(x => x.Category).HasColumnName("category");
            e.Property(x => x.Price).HasColumnName("price");
            e.Property(x => x.ImageUrl).HasColumnName("image_url");
            e.Property(x => x.Features).HasColumnName("features");

            e.HasOne(x => x.Stand)
             .WithMany(s => s.Catalog)
             .HasForeignKey(x => x.StandId)
             .HasConstraintName("fk_catalog_item_stand")
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.StandId, x.Name }).IsUnique();
            e.HasIndex(x => x.StandId);
            e.HasIndex(x => x.Category);
        });

        // visit
        b.Entity<Visit>(e =>
        {
            e.ToTable("visit");
            e.HasKey(x => x.VisitId).HasName("pk_visit");
            e.Property(x => x.VisitId).HasColumnName("visit_id");
            e.Property(x => x.VisitorId).HasColumnName("visitor_id").IsRequired();
            e.Property(x => x.StandId).HasColumnName("stand_id").IsRequired();
            e.Property(x => x.Notes).HasColumnName("notes");
            e.Property(x => x.Rating).HasColumnName("rating");
            e.Property(x => x.Stickers).HasColumnName("stickers").HasColumnType("visit_sticker_enum[]");
            e.Property(x => x.IsFavorite).HasColumnName("is_favorite");
            e.Property(x => x.FollowUp).HasColumnName("follow_up");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");

            e.HasOne(x => x.Visitor)
             .WithMany()
             .HasForeignKey(x => x.VisitorId)
             .HasConstraintName("fk_visit_users")
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Stand)
             .WithMany()
             .HasForeignKey(x => x.StandId)
             .HasConstraintName("fk_visit_stand")
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.VisitorId, x.StandId }).IsUnique();
            e.HasIndex(x => x.VisitorId);
            e.HasIndex(x => x.StandId);
            e.HasIndex(x => x.Rating);
        });

    }
}
