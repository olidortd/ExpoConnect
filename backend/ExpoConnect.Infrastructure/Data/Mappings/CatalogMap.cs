using ExpoConnect.Domain.Expo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpoConnect.Infrastructure.Data.Mappings;

public class CatalogMap : IEntityTypeConfiguration<Catalog>
{
    public void Configure(EntityTypeBuilder<Catalog> e)
    {
        e.ToTable("catalogs");
        e.HasKey(x => x.CatalogId);

        e.Property(x => x.CatalogId)
         .HasColumnName("catalog_id")
         .HasColumnType("uuid")
         .HasDefaultValueSql("uuid_generate_v4()");

        e.Property(x => x.StandId)
            .HasColumnName("stand_id")
            .HasMaxLength(64)
            .IsRequired();

        e.HasOne(x => x.Stand)
             .WithMany(s => s.Catalog)
             .HasForeignKey(x => x.StandId)
             .HasConstraintName("fk_catalog_stand");

        e.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        e.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000)
            .IsRequired();

        e.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        e.HasMany(x => x.Items)
            .WithOne(i => i.Catalog)
            .HasForeignKey(i => i.CatalogId);
    }
}
