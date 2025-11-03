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

        e.Property(x => x.StandId)
            .HasColumnName("stand_id")
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .IsRequired();

        e.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .IsRequired();

        e.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .IsRequired();

        e.Property(x => x.Category)
            .HasColumnName("category")
            .HasColumnType("varchar")
            .HasMaxLength(100);

        e.Property(x => x.Price)
            .HasColumnName("price")
            .HasColumnType("text");

        e.Property(x => x.ImageUrl)
            .HasColumnName("image_url")
            .HasColumnType("text");

        // text[] ← string[]
        e.Property(x => x.Features)
            .HasColumnName("features")
            .HasColumnType("text[]");

        e.HasIndex(x => new { x.StandId, x.Name }).IsUnique();
        e.HasIndex(x => x.StandId);
        e.HasIndex(x => x.Category);

        e.HasOne(x => x.Stand)
            .WithMany(s => s.Catalog)
            .HasForeignKey(x => x.StandId)
            .HasConstraintName("fk_catalog_item_stand")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
