using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class GarmentImageConfiguration : IEntityTypeConfiguration<GarmentImage>
{
    public void Configure(EntityTypeBuilder<GarmentImage> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.Order)
            .IsRequired();

        builder.Property(i => i.IsPrimary)
            .IsRequired();

        builder.HasIndex(i => new { i.GarmentId, i.Order });

        // Soft delete query filter
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
