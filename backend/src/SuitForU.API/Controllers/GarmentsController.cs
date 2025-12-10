using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuitForU.Application.DTOs.Common;
using SuitForU.Application.DTOs.Garments;
using SuitForU.Application.Interfaces;
using System.Security.Claims;

namespace SuitForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GarmentsController : ControllerBase
{
    private readonly IGarmentService _garmentService;
    private readonly ILogger<GarmentsController> _logger;

    public GarmentsController(IGarmentService garmentService, ILogger<GarmentsController> logger)
    {
        _garmentService = garmentService;
        _logger = logger;
    }

    /// <summary>
    /// Récupérer tous les vêtements avec recherche et filtrage
    /// </summary>
    /// <param name="searchDto">Critères de recherche</param>
    /// <returns>Liste paginée de vêtements</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<GarmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllGarments([FromQuery] GarmentSearchDto searchDto)
    {
        try
        {
            var result = await _garmentService.GetAllGarmentsAsync(searchDto);
            return Ok(ApiResponse<PagedResult<GarmentDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting garments");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Récupérer un vêtement par ID
    /// </summary>
    /// <param name="id">ID du vêtement</param>
    /// <returns>Détails du vêtement</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GarmentDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGarmentById(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var garment = await _garmentService.GetGarmentByIdAsync(id, userId);
            
            if (garment == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Vêtement introuvable"));
            }

            return Ok(ApiResponse<GarmentDetailsDto>.SuccessResponse(garment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting garment {GarmentId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Créer un nouveau vêtement
    /// </summary>
    /// <param name="createDto">Données du vêtement</param>
    /// <returns>Vêtement créé</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GarmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateGarment([FromBody] CreateGarmentDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Utilisateur non authentifié"));
            }

            var garment = await _garmentService.CreateGarmentAsync(userId.Value, createDto);
            return CreatedAtAction(
                nameof(GetGarmentById),
                new { id = garment.Id },
                ApiResponse<GarmentDto>.SuccessResponse(garment, "Vêtement créé avec succès"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating garment");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Mettre à jour un vêtement
    /// </summary>
    /// <param name="id">ID du vêtement</param>
    /// <param name="updateDto">Données à mettre à jour</param>
    /// <returns>Vêtement mis à jour</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GarmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateGarment(Guid id, [FromBody] UpdateGarmentDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Utilisateur non authentifié"));
            }

            var garment = await _garmentService.UpdateGarmentAsync(id, userId.Value, updateDto);
            return Ok(ApiResponse<GarmentDto>.SuccessResponse(garment, "Vêtement mis à jour avec succès"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating garment {GarmentId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Supprimer un vêtement
    /// </summary>
    /// <param name="id">ID du vêtement</param>
    /// <returns>Confirmation de suppression</returns>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteGarment(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Utilisateur non authentifié"));
            }

            await _garmentService.DeleteGarmentAsync(id, userId.Value);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Vêtement supprimé avec succès"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting garment {GarmentId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Ajouter une image à un vêtement
    /// </summary>
    /// <param name="id">ID du vêtement</param>
    /// <param name="file">Fichier image</param>
    /// <returns>URL de l'image uploadée</returns>
    [HttpPost("{id:guid}/images")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Aucun fichier fourni"));
            }

            // Vérifier le type de fichier
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Type de fichier non autorisé. Formats acceptés : JPEG, PNG, WebP"));
            }

            // Vérifier la taille (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("La taille du fichier ne doit pas dépasser 5MB"));
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Utilisateur non authentifié"));
            }

            using var stream = file.OpenReadStream();
            var imageUrl = await _garmentService.UploadGarmentImageAsync(id, userId.Value, stream, file.FileName);
            
            return Ok(ApiResponse<string>.SuccessResponse(imageUrl, "Image ajoutée avec succès"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for garment {GarmentId}", id);
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Récupérer les vêtements de l'utilisateur connecté
    /// </summary>
    /// <returns>Liste des vêtements</returns>
    [HttpGet("my-garments")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<GarmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyGarments()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("Utilisateur non authentifié"));
            }

            var garments = await _garmentService.GetUserGarmentsAsync(userId.Value);
            return Ok(ApiResponse<List<GarmentDto>>.SuccessResponse(garments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user garments");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}
