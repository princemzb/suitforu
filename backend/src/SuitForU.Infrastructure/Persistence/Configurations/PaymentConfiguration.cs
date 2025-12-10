using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Method)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.PaymentIntentId)
            .HasMaxLength(255);

        builder.HasIndex(p => p.PaymentIntentId);

        builder.Property(p => p.TransactionId)
            .HasMaxLength(255);

        builder.Property(p => p.StripeChargeId)
            .HasMaxLength(255);

        builder.Property(p => p.PayPalOrderId)
            .HasMaxLength(255);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        // Indexes for queries
        builder.HasIndex(p => p.Status);

        // Soft delete query filter
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
