using ExpoConnect.Domain.Expo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpoConnect.Infrastructure.Data.Mappings;

public class VisitMap : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> e)
    {
        e.ToTable("visit");

        e.HasKey(x => x.VisitId).HasName("pk_visit");
        e.Property(x => x.VisitId)
            .HasColumnName("visit_id")
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuid_generate_v4()");

        e.Property(x => x.VisitorId)
            .HasColumnName("visitor_id")
            .HasColumnType("text")
            .IsRequired();

        e.Property(x => x.StandId)
            .HasColumnName("stand_id")
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .IsRequired();

        e.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        e.Property(x => x.Rating)
            .HasColumnName("rating")
            .HasColumnType("int4");

        e.Property(v => v.Stickers)
             .HasColumnName("stickers")
             .HasColumnType("visit_sticker[]")
             .IsRequired(false);

        e.Property(x => x.IsFavorite)
            .HasColumnName("is_favorite")
            .HasColumnType("bool")
            .HasDefaultValue(false)
            .IsRequired();

        e.Property(x => x.FollowUp)
            .HasColumnName("follow_up")
            .HasColumnType("bool")
            .HasDefaultValue(false)
            .IsRequired();

        e.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()");

        e.HasIndex(x => new { x.VisitorId, x.StandId }).IsUnique();
        e.HasIndex(x => x.VisitorId);
        e.HasIndex(x => x.StandId);
        e.HasIndex(x => x.Rating);

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
    }
}
