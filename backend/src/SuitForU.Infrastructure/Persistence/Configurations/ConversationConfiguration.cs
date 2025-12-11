using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuitForU.Domain.Entities;

namespace SuitForU.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.LastMessageContent)
            .HasMaxLength(500);

        // Une conversation appartient à un vêtement
        builder.HasOne(c => c.Garment)
            .WithMany()
            .HasForeignKey(c => c.GarmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Une conversation a deux participants
        builder.HasOne(c => c.User1)
            .WithMany()
            .HasForeignKey(c => c.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.User2)
            .WithMany()
            .HasForeignKey(c => c.User2Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour rechercher rapidement les conversations d'un utilisateur
        builder.HasIndex(c => c.User1Id);
        builder.HasIndex(c => c.User2Id);
        builder.HasIndex(c => c.GarmentId);
        builder.HasIndex(c => c.LastMessageAt);

        // Index composite unique pour éviter les doublons de conversation
        // Utilise une fonction SQL pour toujours avoir le plus petit UserId en premier
        builder.HasIndex(c => new { c.GarmentId, c.User1Id, c.User2Id })
            .IsUnique()
            .HasDatabaseName("IX_Conversations_GarmentId_Users");
    }
}
