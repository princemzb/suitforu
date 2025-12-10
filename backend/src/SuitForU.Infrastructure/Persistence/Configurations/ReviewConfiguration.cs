using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        // Indexes for queries
        builder.HasIndex(r => r.GarmentId);
        builder.HasIndex(r => r.ReviewerId);
        builder.HasIndex(r => r.ReviewedUserId);
        builder.HasIndex(r => r.RentalId)
            .IsUnique();
        builder.HasIndex(r => r.Rating);

        // Soft delete query filter
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
