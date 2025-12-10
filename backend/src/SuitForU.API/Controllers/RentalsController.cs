using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuitForU.Application.DTOs.Common;
using SuitForU.Application.DTOs.Rentals;
using SuitForU.Application.Interfaces;
using System.Security.Claims;

namespace SuitForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly ILogger<RentalsController> _logger;

    public RentalsController(IRentalService rentalService, ILogger<RentalsController> logger)
    {
        _rentalService = rentalService;
        _logger = logger;
    }

    /// <summary>
    /// Créer une demande de réservation
    /// </summary>
    /// <param name="createDto">Données de la réservation</param>
    /// <returns>Réservation créée</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<RentalDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var rental = await _rentalService.CreateRentalAsync(userId.Value, createDto);
            
            return CreatedAtAction(
                nameof(GetRentalById),
                new { id = rental.Id },
                ApiResponse<RentalDto>.SuccessResponse(rental, "Rental request created successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rental");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Récupérer les détails d'une réservation
    /// </summary>
    /// <param name="id">ID de la réservation</param>
    /// <returns>Détails de la réservation</returns>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<RentalDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRentalById(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var rental = await _rentalService.GetRentalByIdAsync(id, userId.Value);
            
            if (rental == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Rental not found"));
            }

            return Ok(ApiResponse<RentalDetailsDto>.SuccessResponse(rental));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rental {RentalId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Récupérer mes réservations (en tant que locataire)
    /// </summary>
    /// <returns>Liste des réservations</returns>
    [HttpGet("my-rentals")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<RentalDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyRentals()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var rentals = await _rentalService.GetUserRentalsAsync(userId.Value);
            return Ok(ApiResponse<List<RentalDto>>.SuccessResponse(rentals));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user rentals");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Récupérer les réservations de mes vêtements (en tant que propriétaire)
    /// </summary>
    /// <returns>Liste des réservations</returns>
    [HttpGet("owner-rentals")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<RentalDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOwnerRentals()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var rentals = await _rentalService.GetOwnerRentalsAsync(userId.Value);
            return Ok(ApiResponse<List<RentalDto>>.SuccessResponse(rentals));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting owner rentals");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Accepter une demande de réservation (propriétaire)
    /// </summary>
    /// <param name="id">ID de la réservation</param>
    /// <returns>Réservation acceptée</returns>
    [HttpPost("{id:guid}/accept")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<RentalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AcceptRental(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var rental = await _rentalService.AcceptRentalAsync(id, userId.Value);
            return Ok(ApiResponse<RentalDto>.SuccessResponse(rental, "Rental accepted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting rental {RentalId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Confirmer la réservation après paiement (locataire)
    /// </summary>
    /// <param name="id">ID de la réservation</param>
    /// <returns>Réservation confirmée</returns>
    [HttpPost("{id:guid}/confirm")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<RentalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ConfirmRental(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var rental = await _rentalService.ConfirmRentalAsync(id, userId.Value);
            return Ok(ApiResponse<RentalDto>.SuccessResponse(rental, "Rental confirmed successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming rental {RentalId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Prolonger une réservation
    /// </summary>
    /// <param name="id">ID de la réservation</param>
    /// <param name="extendDto">Nouvelles dates</param>
    /// <returns>Réservation prolongée</returns>
    [HttpPost("{id:guid}/extend")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<RentalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ExtendRental(Guid id, [FromBody] ExtendRentalDto extendDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var rental = await _rentalService.ExtendRentalAsync(id, userId.Value, extendDto);
            return Ok(ApiResponse<RentalDto>.SuccessResponse(rental, "Rental extended successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extending rental {RentalId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Annuler une réservation
    /// </summary>
    /// <param name="id">ID de la réservation</param>
    /// <param name="cancelDto">Raison de l'annulation</param>
    /// <returns>Confirmation d'annulation</returns>
    [HttpPost("{id:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CancelRental(Guid id, [FromBody] CancelRentalDto cancelDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            await _rentalService.CancelRentalAsync(id, userId.Value, cancelDto);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Rental cancelled successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling rental {RentalId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}
