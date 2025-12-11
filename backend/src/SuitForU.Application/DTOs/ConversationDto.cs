namespace SuitForU.Application.DTOs;

/// <summary>
/// DTO pour une conversation
/// </summary>
public class ConversationDto
{
    public Guid Id { get; set; }
    public Guid GarmentId { get; set; }
    public string GarmentTitle { get; set; } = string.Empty;
    public string? GarmentImageUrl { get; set; }
    public Guid OtherParticipantId { get; set; }
    public string OtherParticipantName { get; set; } = string.Empty;
    public string? OtherParticipantProfilePicture { get; set; }
    public string? LastMessageContent { get; set; }
    public Guid? LastMessageSenderId { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
