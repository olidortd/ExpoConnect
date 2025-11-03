using ExpoConnect.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpoConnect.Infrastructure.Data.Mappings
{
    public class UserCredentialMap : IEntityTypeConfiguration<UserCredential>
    {
        public void Configure(EntityTypeBuilder<UserCredential> e)
        {
            e.ToTable("user_credentials");
            e.HasKey(x => x.UserId).HasName("pk_user_credentials");
            e.Property(x => x.UserId).HasColumnName("user_id").HasColumnType("text").IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasColumnType("text").IsRequired();
        }
    }
}
