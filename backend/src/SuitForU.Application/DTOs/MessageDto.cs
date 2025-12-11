namespace SuitForU.Application.DTOs;

/// <summary>
/// DTO pour un message
/// </summary>
public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string? SenderProfilePicture { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO pour l'envoi d'un message
/// </summary>
public class SendMessageDto
{
    public string Content { get; set; } = string.Empty;
}
