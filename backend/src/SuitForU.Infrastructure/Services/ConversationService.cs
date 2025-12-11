using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SuitForU.Application.DTOs;
using SuitForU.Application.Services;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Interfaces;
using SuitForU.Infrastructure.Persistence;

namespace SuitForU.Infrastructure.Services;

public class ConversationService : IConversationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ConversationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ConversationDto> GetOrCreateConversationAsync(Guid garmentId, Guid currentUserId)
    {
        // Récupérer le vêtement
        var garment = await _unitOfWork.Garments.GetByIdAsync(garmentId);
        if (garment == null || garment.IsDeleted)
            throw new KeyNotFoundException("Garment not found");

        var ownerId = garment.OwnerId;
        if (ownerId == currentUserId)
            throw new InvalidOperationException("Cannot create conversation with yourself");

        // Normaliser les IDs (toujours User1Id < User2Id pour éviter les doublons)
        var user1Id = currentUserId.CompareTo(ownerId) < 0 ? currentUserId : ownerId;
        var user2Id = currentUserId.CompareTo(ownerId) < 0 ? ownerId : currentUserId;

        // Chercher une conversation existante
        var allConversations = await _unitOfWork.Conversations.GetAllAsync();
        var conversation = allConversations.FirstOrDefault(c => c.GarmentId == garmentId && 
                           c.User1Id == user1Id && 
                           c.User2Id == user2Id &&
                           !c.IsDeleted);

        // Créer la conversation si elle n'existe pas
        if (conversation == null)
        {
            conversation = new Conversation
            {
                GarmentId = garmentId,
                User1Id = user1Id,
                User2Id = user2Id
            };

            await _unitOfWork.Conversations.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();
        }

        return await BuildConversationDtoAsync(conversation, currentUserId);
    }

    public async Task<MessageDto> SendMessageAsync(Guid conversationId, SendMessageDto dto, Guid senderId)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new ArgumentException("Message content cannot be empty");

        if (dto.Content.Length > 2000)
            throw new ArgumentException("Message content is too long (max 2000 characters)");

        // Vérifier que la conversation existe et que l'utilisateur y participe
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);

        if (conversation == null || conversation.IsDeleted)
            throw new KeyNotFoundException("Conversation not found");

        if (!conversation.HasParticipant(senderId))
            throw new UnauthorizedAccessException("You are not a participant of this conversation");

        // Créer le message
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = dto.Content.Trim(),
            IsRead = false
        };

        await _unitOfWork.Messages.AddAsync(message);

        // Mettre à jour le dernier message de la conversation
        conversation.LastMessageContent = dto.Content.Length > 100 
            ? dto.Content.Substring(0, 100) + "..." 
            : dto.Content;
        conversation.LastMessageSenderId = senderId;
        conversation.LastMessageAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        // Récupérer le sender pour le DTO
        var sender = await _unitOfWork.Users.GetByIdAsync(senderId);
        
        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = $"{sender!.FirstName} {sender.LastName}",
            SenderProfilePicture = sender.ProfilePictureUrl,
            Content = message.Content,
            IsRead = message.IsRead,
            ReadAt = message.ReadAt,
            CreatedAt = message.CreatedAt
        };
    }

    public async Task<List<MessageDto>> GetConversationMessagesAsync(Guid conversationId, Guid currentUserId)
    {
        // Vérifier que la conversation existe et que l'utilisateur y participe
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);

        if (conversation == null || conversation.IsDeleted)
            throw new KeyNotFoundException("Conversation not found");

        if (!conversation.HasParticipant(currentUserId))
            throw new UnauthorizedAccessException("You are not a participant of this conversation");

        // Récupérer tous les messages
        var allMessages = await _unitOfWork.Messages.GetAllAsync();
        var messages = allMessages.Where(m => m.ConversationId == conversationId && !m.IsDeleted);

        var messagesList = messages.OrderBy(m => m.CreatedAt).ToList();

        // Construire les DTOs avec les infos des senders
        var messageDtos = new List<MessageDto>();
        foreach (var message in messagesList)
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId);
            messageDtos.Add(new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = $"{sender!.FirstName} {sender.LastName}",
                SenderProfilePicture = sender.ProfilePictureUrl,
                Content = message.Content,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                CreatedAt = message.CreatedAt
            });
        }

        return messageDtos;
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync(Guid userId)
    {
        // Récupérer toutes les conversations où l'utilisateur participe
        var allConversations = await _unitOfWork.Conversations.GetAllAsync();
        var conversations = allConversations.Where(c => (c.User1Id == userId || c.User2Id == userId) && !c.IsDeleted);

        var conversationDtos = new List<ConversationDto>();
        foreach (var conversation in conversations.OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt))
        {
            var dto = await BuildConversationDtoAsync(conversation, userId);
            conversationDtos.Add(dto);
        }

        return conversationDtos;
    }

    public async Task MarkMessageAsReadAsync(Guid messageId, Guid currentUserId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
        
        if (message == null || message.IsDeleted)
            throw new KeyNotFoundException("Message not found");

        // Vérifier que l'utilisateur est le destinataire (pas l'expéditeur)
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(message.ConversationId);
        
        if (conversation == null || !conversation.HasParticipant(currentUserId))
            throw new UnauthorizedAccessException("Access denied");

        if (message.SenderId == currentUserId)
            return; // L'expéditeur ne peut pas marquer son propre message comme lu

        if (!message.IsRead)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task MarkConversationAsReadAsync(Guid conversationId, Guid currentUserId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
        
        if (conversation == null || conversation.IsDeleted)
            throw new KeyNotFoundException("Conversation not found");

        if (!conversation.HasParticipant(currentUserId))
            throw new UnauthorizedAccessException("Access denied");

        // Marquer tous les messages non lus comme lus (sauf ceux envoyés par l'utilisateur lui-même)
        var allMessages = await _unitOfWork.Messages.GetAllAsync();
        var unreadMessages = allMessages.Where(m => m.ConversationId == conversationId && 
                              !m.IsRead && 
                              m.SenderId != currentUserId &&
                              !m.IsDeleted);

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<ConversationDto> BuildConversationDtoAsync(Conversation conversation, Guid currentUserId)
    {
        // Récupérer le vêtement
        var garment = await _unitOfWork.Garments.GetByIdAsync(conversation.GarmentId);
        
        // Récupérer l'autre participant
        var otherParticipantId = conversation.GetOtherParticipantId(currentUserId);
        var otherParticipant = await _unitOfWork.Users.GetByIdAsync(otherParticipantId);

        // Récupérer l'image principale du vêtement
        var allImages = await _unitOfWork.GarmentImages.GetAllAsync();
        var images = allImages.Where(img => img.GarmentId == conversation.GarmentId && !img.IsDeleted);
        var primaryImage = images.FirstOrDefault(img => img.IsPrimary) ?? images.FirstOrDefault();

        // Compter les messages non lus
        var allUnreadMessages = await _unitOfWork.Messages.GetAllAsync();
        var unreadMessages = allUnreadMessages.Where(m => m.ConversationId == conversation.Id && 
                              !m.IsRead && 
                              m.SenderId != currentUserId &&
                              !m.IsDeleted);

        return new ConversationDto
        {
            Id = conversation.Id,
            GarmentId = conversation.GarmentId,
            GarmentTitle = garment?.Title ?? "Unknown",
            GarmentImageUrl = primaryImage?.ImageUrl,
            OtherParticipantId = otherParticipantId,
            OtherParticipantName = $"{otherParticipant!.FirstName} {otherParticipant.LastName}",
            OtherParticipantProfilePicture = otherParticipant.ProfilePictureUrl,
            LastMessageContent = conversation.LastMessageContent,
            LastMessageSenderId = conversation.LastMessageSenderId,
            LastMessageAt = conversation.LastMessageAt,
            UnreadCount = unreadMessages.Count(),
            CreatedAt = conversation.CreatedAt
        };
    }
}
