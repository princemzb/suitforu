using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.DepositAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.DailyPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.EndDate)
            .IsRequired();

        builder.Property(r => r.DurationDays)
            .IsRequired();

        builder.Property(r => r.CancellationReason)
            .HasMaxLength(1000);

        // Relations
        builder.HasMany(r => r.Payments)
            .WithOne(p => p.Rental)
            .HasForeignKey(p => p.RentalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Review)
            .WithOne(rv => rv.Rental)
            .HasForeignKey<Review>(rv => rv.RentalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for queries
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.StartDate);
        builder.HasIndex(r => r.EndDate);
        builder.HasIndex(r => new { r.GarmentId, r.StartDate, r.EndDate });

        // Soft delete query filter
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
