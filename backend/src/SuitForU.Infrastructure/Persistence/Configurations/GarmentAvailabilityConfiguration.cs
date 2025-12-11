using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class GarmentAvailabilityConfiguration : IEntityTypeConfiguration<GarmentAvailability>
{
    public void Configure(EntityTypeBuilder<GarmentAvailability> builder)
    {
        builder.HasKey(ga => ga.Id);

        builder.Property(ga => ga.Date)
            .IsRequired()
            .HasColumnType("date"); // Stocke uniquement la date, pas l'heure

        builder.Property(ga => ga.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ga => ga.BlockedReason)
            .HasConversion<int>();

        builder.Property(ga => ga.Notes)
            .HasMaxLength(500);

        // Une disponibilité appartient à un vêtement
        builder.HasOne(ga => ga.Garment)
            .WithMany()
            .HasForeignKey(ga => ga.GarmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Une disponibilité peut être liée à une location
        builder.HasOne(ga => ga.Rental)
            .WithMany()
            .HasForeignKey(ga => ga.RentalId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index pour rechercher rapidement les disponibilités d'un vêtement
        builder.HasIndex(ga => ga.GarmentId);
        builder.HasIndex(ga => ga.Date);
        builder.HasIndex(ga => new { ga.GarmentId, ga.Date });

        // Index unique pour éviter les doublons (1 seule entrée par garment + date)
        builder.HasIndex(ga => new { ga.GarmentId, ga.Date })
            .IsUnique()
            .HasDatabaseName("IX_GarmentAvailabilities_GarmentId_Date_Unique");
    }
}
