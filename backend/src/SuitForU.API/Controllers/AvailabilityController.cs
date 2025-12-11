using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuitForU.Application.DTOs;
using SuitForU.Application.Services;
using System.Security.Claims;

namespace SuitForU.API.Controllers;

[ApiController]
[Route("api/garments/{garmentId}/availability")]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    /// <summary>
    /// Récupère le calendrier de disponibilité d'un vêtement (3 mois par défaut)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<AvailabilityCalendarDto>> GetAvailabilityCalendar(
        Guid garmentId,
        [FromQuery] int months = 3)
    {
        if (months < 1 || months > 12)
            return BadRequest("Months must be between 1 and 12");

        var calendar = await _availabilityService.GetAvailabilityCalendarAsync(garmentId, months);
        return Ok(calendar);
    }

    /// <summary>
    /// Vérifie si une période spécifique est disponible
    /// </summary>
    [HttpGet("check")]
    [AllowAnonymous]
    public async Task<ActionResult<CheckAvailabilityDto>> CheckAvailability(
        Guid garmentId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _availabilityService.CheckAvailabilityAsync(garmentId, startDate, endDate);
        return Ok(result);
    }

    /// <summary>
    /// Bloque manuellement des dates (propriétaire uniquement)
    /// </summary>
    [HttpPost("block")]
    [Authorize]
    public async Task<IActionResult> BlockDates(Guid garmentId, [FromBody] BlockDatesDto dto)
    {
        var userId = GetCurrentUserId();
        await _availabilityService.BlockDatesAsync(garmentId, dto, userId);
        return NoContent();
    }

    /// <summary>
    /// Débloque manuellement des dates (propriétaire uniquement)
    /// </summary>
    [HttpDelete("unblock")]
    [Authorize]
    public async Task<IActionResult> UnblockDates(Guid garmentId, [FromBody] UnblockDatesDto dto)
    {
        var userId = GetCurrentUserId();
        await _availabilityService.UnblockDatesAsync(garmentId, dto, userId);
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
