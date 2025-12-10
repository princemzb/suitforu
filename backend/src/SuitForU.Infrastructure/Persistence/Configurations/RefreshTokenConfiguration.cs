using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(rt => rt.CreatedByIp)
            .IsRequired()
            .HasMaxLength(45); // IPv6 max length

        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(45);

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        // Relation avec User
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index pour améliorer les performances
        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.HasIndex(rt => rt.UserId);

        builder.HasIndex(rt => rt.ExpiresAt);

        // Ignorer les propriétés calculées
        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsActive);
    }
}
