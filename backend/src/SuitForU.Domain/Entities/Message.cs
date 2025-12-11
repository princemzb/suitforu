using SuitForU.Domain.Common;

namespace SuitForU.Domain.Entities;

/// <summary>
/// Représente un message dans une conversation
/// </summary>
public class Message : BaseEntity
{
    /// <summary>
    /// ID de la conversation à laquelle appartient ce message
    /// </summary>
    public Guid ConversationId { get; set; }
    
    /// <summary>
    /// Navigation vers la conversation
    /// </summary>
    public Conversation Conversation { get; set; } = null!;
    
    /// <summary>
    /// ID de l'expéditeur du message
    /// </summary>
    public Guid SenderId { get; set; }
    
    /// <summary>
    /// Navigation vers l'expéditeur
    /// </summary>
    public User Sender { get; set; } = null!;
    
    /// <summary>
    /// Contenu du message
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Indique si le message a été lu par le destinataire
    /// </summary>
    public bool IsRead { get; set; }
    
    /// <summary>
    /// Date et heure de lecture du message
    /// </summary>
    public DateTime? ReadAt { get; set; }
}
