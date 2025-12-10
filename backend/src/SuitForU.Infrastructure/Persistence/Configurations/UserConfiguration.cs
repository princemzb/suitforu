using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);

        builder.Property(u => u.AuthProvider)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(u => u.ExternalProviderId)
            .HasMaxLength(255);

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(500);

        // Relations
        builder.HasMany(u => u.Garments)
            .WithOne(g => g.Owner)
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.RentalsAsRenter)
            .WithOne(r => r.Renter)
            .HasForeignKey(r => r.RenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.RentalsAsOwner)
            .WithOne(r => r.Owner)
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Payments)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReviewsGiven)
            .WithOne(r => r.Reviewer)
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReviewsReceived)
            .WithOne(r => r.ReviewedUser)
            .HasForeignKey(r => r.ReviewedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete query filter
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
