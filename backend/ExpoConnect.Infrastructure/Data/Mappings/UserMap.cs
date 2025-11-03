using ExpoConnect.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpoConnect.Infrastructure.Data.Mappings;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> e)
    {
        e.ToTable("users");

        e.HasKey(x => x.UserId).HasName("pk_users");

        e.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("text")
            .IsRequired();

        e.Property(x => x.Email)
            .HasColumnName("email")
            .HasColumnType("varchar")
            .HasMaxLength(255)
            .IsRequired();

        e.Property(x => x.DisplayName)
            .HasColumnName("display_name")
            .HasColumnType("varchar")
            .HasMaxLength(200);

        e.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasColumnType("varchar")
            .HasMaxLength(30);

        e.Property(x => x.Company)
            .HasColumnName("company")
            .HasColumnType("varchar")
            .HasMaxLength(200);

        e.Property(x => x.Role)
            .HasColumnName("role")
            .HasConversion<string>()      // enum <-> string
            .HasColumnType("text")
            .IsRequired();

        e.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("bool")
            .HasDefaultValue(true)
            .IsRequired();

        e.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()");

        e.Property(x => x.PasswordHash)
            .HasColumnName("password_hash").IsRequired();

        e.Property(x => x.RefreshTokenHash)
            .HasColumnName("refresh_token_hash");

        e.Property(x => x.RefreshTokenExpiresAtUtc)
            .HasColumnName("refresh_token_expires_at_utc");


        e.HasIndex(x => x.Email).IsUnique();
        e.HasIndex(x => x.Role);
        e.HasIndex(x => x.IsActive);
    }
}
