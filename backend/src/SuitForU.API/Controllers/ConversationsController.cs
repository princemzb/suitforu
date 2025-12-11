using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuitForU.Application.DTOs;
using SuitForU.Application.Services;
using System.Security.Claims;

namespace SuitForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationsController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    /// <summary>
    /// Récupère ou crée une conversation pour un vêtement
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ConversationDto>> GetOrCreateConversation([FromBody] CreateConversationRequest request)
    {
        var userId = GetCurrentUserId();
        var conversation = await _conversationService.GetOrCreateConversationAsync(request.GarmentId, userId);
        return Ok(conversation);
    }

    /// <summary>
    /// Récupère toutes les conversations de l'utilisateur
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ConversationDto>>> GetMyConversations()
    {
        var userId = GetCurrentUserId();
        var conversations = await _conversationService.GetUserConversationsAsync(userId);
        return Ok(conversations);
    }

    /// <summary>
    /// Récupère tous les messages d'une conversation
    /// </summary>
    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetConversationMessages(Guid conversationId)
    {
        var userId = GetCurrentUserId();
        var messages = await _conversationService.GetConversationMessagesAsync(conversationId, userId);
        return Ok(messages);
    }

    /// <summary>
    /// Envoie un message dans une conversation
    /// </summary>
    [HttpPost("{conversationId}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(Guid conversationId, [FromBody] SendMessageDto dto)
    {
        var userId = GetCurrentUserId();
        var message = await _conversationService.SendMessageAsync(conversationId, dto, userId);
        return Ok(message);
    }

    /// <summary>
    /// Marque un message comme lu
    /// </summary>
    [HttpPut("messages/{messageId}/read")]
    public async Task<IActionResult> MarkMessageAsRead(Guid messageId)
    {
        var userId = GetCurrentUserId();
        await _conversationService.MarkMessageAsReadAsync(messageId, userId);
        return NoContent();
    }

    /// <summary>
    /// Marque tous les messages d'une conversation comme lus
    /// </summary>
    [HttpPut("{conversationId}/read")]
    public async Task<IActionResult> MarkConversationAsRead(Guid conversationId)
    {
        var userId = GetCurrentUserId();
        await _conversationService.MarkConversationAsReadAsync(conversationId, userId);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return userId;
    }
}

public class CreateConversationRequest
{
    public Guid GarmentId { get; set; }
}
