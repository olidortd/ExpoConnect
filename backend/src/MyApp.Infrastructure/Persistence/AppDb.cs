using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using ExpoConnect.Domain.Users;
using ExpoConnect.Domain.Auth;
using ExpoConnect.Domain.Expo;

namespace ExpoConnect.Infrastructure.Persistence;

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }

    // DbSets for your entities:
    public DbSet<User> Users => Set<User>();
    public DbSet<CatalogItem> Items => Set<CatalogItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Example configurations
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.UserId);
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<CatalogItem>(e =>
        {
            e.HasKey(i => i.ItemId);
            e.Property(i => i.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasIndex(r => r.Token).IsUnique();
        });
    }
}
