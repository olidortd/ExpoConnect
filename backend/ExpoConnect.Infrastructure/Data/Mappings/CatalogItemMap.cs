using ExpoConnect.Domain.Expo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpoConnect.Infrastructure.Data.Mappings;

public class CatalogItemMap : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> e)
    {
        e.ToTable("catalog_item");

        e.HasKey(x => x.ItemId).HasName("pk_catalog_item");

        e.Property(x => x.ItemId)
            .HasColumnName("item_id")
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuid_generate_v4()");

        e.Property(x => x.CatalogId)
            .HasColumnName("catalog_id")
            .HasColumnType("uuid")
            .IsRequired();

        e.HasOne(x => x.Catalog)
            .WithMany(c => c.Items)
            .HasForeignKey(x => x.CatalogId)
            .HasConstraintName("fk_catalog_item_catalog")
            .OnDelete(DeleteBehavior.Cascade);

        e.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .IsRequired();

        e.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired(false);

        e.Property(x => x.Category)
            .HasColumnName("category")
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

        e.Property(x => x.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(12,2)")
            .IsRequired();

        e.Property(x => x.ImageUrl)
            .HasColumnName("image_url")
            .HasColumnType("text")
            .IsRequired(false);

        e.Property(x => x.Features)
            .HasColumnName("features")
            .HasColumnType("text[]")
            .IsRequired(false);

        e.HasIndex(x => new { x.CatalogId, x.Name }).IsUnique();
        e.HasIndex(x => x.CatalogId);
        e.HasIndex(x => x.Category);
    }
}
