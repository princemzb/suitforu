using SuitForU.Application.DTOs;
using SuitForU.Domain.Entities;

namespace SuitForU.Application.Services;

/// <summary>
/// Service de gestion des conversations et messages
/// </summary>
public interface IConversationService
{
    /// <summary>
    /// Récupère ou crée une conversation entre deux utilisateurs pour un vêtement
    /// </summary>
    Task<ConversationDto> GetOrCreateConversationAsync(Guid garmentId, Guid currentUserId);
    
    /// <summary>
    /// Envoie un message dans une conversation
    /// </summary>
    Task<MessageDto> SendMessageAsync(Guid conversationId, SendMessageDto dto, Guid senderId);
    
    /// <summary>
    /// Récupère tous les messages d'une conversation
    /// </summary>
    Task<List<MessageDto>> GetConversationMessagesAsync(Guid conversationId, Guid currentUserId);
    
    /// <summary>
    /// Récupère toutes les conversations d'un utilisateur
    /// </summary>
    Task<List<ConversationDto>> GetUserConversationsAsync(Guid userId);
    
    /// <summary>
    /// Marque un message comme lu
    /// </summary>
    Task MarkMessageAsReadAsync(Guid messageId, Guid currentUserId);
    
    /// <summary>
    /// Marque tous les messages d'une conversation comme lus
    /// </summary>
    Task MarkConversationAsReadAsync(Guid conversationId, Guid currentUserId);
}
