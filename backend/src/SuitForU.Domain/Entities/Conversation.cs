using SuitForU.Domain.Common;

namespace SuitForU.Domain.Entities;

/// <summary>
/// Représente une conversation entre deux utilisateurs concernant un vêtement spécifique.
/// Une seule conversation peut exister pour une combinaison unique de (Garment, User1, User2).
/// </summary>
public class Conversation : BaseEntity
{
    /// <summary>
    /// ID du vêtement concerné par la conversation
    /// </summary>
    public Guid GarmentId { get; set; }
    
    /// <summary>
    /// Navigation vers le vêtement
    /// </summary>
    public Garment Garment { get; set; } = null!;
    
    /// <summary>
    /// ID du premier utilisateur (généralement le propriétaire du vêtement)
    /// </summary>
    public Guid User1Id { get; set; }
    
    /// <summary>
    /// Navigation vers le premier utilisateur
    /// </summary>
    public User User1 { get; set; } = null!;
    
    /// <summary>
    /// ID du second utilisateur (généralement l'intéressé)
    /// </summary>
    public Guid User2Id { get; set; }
    
    /// <summary>
    /// Navigation vers le second utilisateur
    /// </summary>
    public User User2 { get; set; } = null!;
    
    /// <summary>
    /// Date et heure du dernier message envoyé
    /// </summary>
    public DateTime? LastMessageAt { get; set; }
    
    /// <summary>
    /// Contenu du dernier message (pour affichage rapide dans la liste)
    /// </summary>
    public string? LastMessageContent { get; set; }
    
    /// <summary>
    /// ID de l'auteur du dernier message
    /// </summary>
    public Guid? LastMessageSenderId { get; set; }
    
    /// <summary>
    /// Collection des messages de cette conversation
    /// </summary>
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    
    /// <summary>
    /// Vérifie si un utilisateur fait partie de cette conversation
    /// </summary>
    public bool HasParticipant(Guid userId)
    {
        return User1Id == userId || User2Id == userId;
    }
    
    /// <summary>
    /// Récupère l'ID de l'autre participant
    /// </summary>
    public Guid GetOtherParticipantId(Guid currentUserId)
    {
        return User1Id == currentUserId ? User2Id : User1Id;
    }
}
