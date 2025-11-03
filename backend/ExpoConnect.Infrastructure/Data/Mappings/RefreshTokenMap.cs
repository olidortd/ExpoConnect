// Data/Mappings/RefreshTokenMap.cs
using ExpoConnect.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpoConnect.Infrastructure.Data.Mappings
{
    public class RefreshTokenMap : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> e)
        {
            e.ToTable("auth_refresh_token");
            e.HasKey(x => x.Id).HasName("pk_auth_refresh_token");
            e.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid");
            e.Property(x => x.Token).HasColumnName("token").HasColumnType("text").IsRequired();
            e.Property(x => x.ExpiresAtUtc).HasColumnName("expires_at_utc").HasColumnType("timestamptz");
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz");
            e.Property(x => x.RevokedAtUtc).HasColumnName("revoked_at_utc").HasColumnType("timestamptz");
            e.Property(x => x.ReplacedByToken).HasColumnName("replaced_by_token").HasColumnType("text");
            e.Property(x => x.UserId).HasColumnName("user_id").HasColumnType("text").IsRequired();
            e.HasIndex(x => x.Token).IsUnique();
        }
    }
}
