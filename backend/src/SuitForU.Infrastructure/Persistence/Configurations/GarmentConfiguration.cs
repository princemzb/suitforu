using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class GarmentConfiguration : IEntityTypeConfiguration<Garment>
{
    public void Configure(EntityTypeBuilder<Garment> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(g => g.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(g => g.Condition)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(g => g.Size)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(g => g.Brand)
            .HasMaxLength(100);

        builder.Property(g => g.Color)
            .HasMaxLength(50);

        builder.Property(g => g.DailyPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(g => g.DepositAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(g => g.PickupAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(g => g.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(g => g.City);

        builder.Property(g => g.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(g => g.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Latitude);

        builder.Property(g => g.Longitude);

        // Relations
        builder.HasMany(g => g.Images)
            .WithOne(i => i.Garment)
            .HasForeignKey(i => i.GarmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Rentals)
            .WithOne(r => r.Garment)
            .HasForeignKey(r => r.GarmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(g => g.Reviews)
            .WithOne(r => r.Garment)
            .HasForeignKey(r => r.GarmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for search
        builder.HasIndex(g => g.DailyPrice);
        builder.HasIndex(g => g.IsAvailable);

        // Soft delete query filter
        builder.HasQueryFilter(g => !g.IsDeleted);
    }
}
