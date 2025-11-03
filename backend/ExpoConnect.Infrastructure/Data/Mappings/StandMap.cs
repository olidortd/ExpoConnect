using ExpoConnect.Domain.Expo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpoConnect.Infrastructure.Data.Mappings;

public class StandMap : IEntityTypeConfiguration<Stand>
{
    public void Configure(EntityTypeBuilder<Stand> e)
    {
        e.ToTable("stand");

        e.HasKey(x => x.QrCode).HasName("pk_stand");

        e.Property(x => x.QrCode)
            .HasColumnName("qr_code")
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .IsRequired();

        e.Property(x => x.StandNumber)
            .HasColumnName("stand_number")
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

        e.Property(x => x.CompanyName)
            .HasColumnName("company_name")
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .IsRequired();

        e.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        e.Property(x => x.Industry)
            .HasColumnName("industry")
            .HasConversion<string>()      // enum <-> string
            .HasColumnType("text");
        e.Property(x => x.ContactEmail)
            .HasColumnName("contact_email")
            .HasColumnType("varchar")
            .HasMaxLength(255);

        e.Property(x => x.ContactPhone)
            .HasColumnName("contact_phone")
            .HasColumnType("varchar")
            .HasMaxLength(30);

        e.Property(x => x.Website).HasColumnName("website").HasColumnType("text");
        e.Property(x => x.LogoUrl).HasColumnName("logo_url").HasColumnType("text");
        e.Property(x => x.BannerUrl).HasColumnName("banner_url").HasColumnType("text");

        e.Property(x => x.ExhibitorId)
            .HasColumnName("exhibitor_id")
            .HasColumnType("text")
            .IsRequired();

        e.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz");

        e.HasIndex(x => x.ExhibitorId);
        e.HasIndex(x => x.StandNumber);

        e.HasOne(x => x.Exhibitor)
            .WithMany()
            .HasForeignKey(x => x.ExhibitorId)
            .HasConstraintName("fk_stand_users")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
