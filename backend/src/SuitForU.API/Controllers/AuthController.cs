using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuitForU.Application.DTOs.Auth;
using SuitForU.Application.DTOs.Common;
using SuitForU.Application.Interfaces;
using System.Security.Claims;

namespace SuitForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    private string? GetIpAddress()
    {
        // Récupérer l'IP depuis les headers (si derrière un proxy)
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].ToString().Split(',').FirstOrDefault()?.Trim();
        }
        
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// Inscription d'un nouvel utilisateur
    /// </summary>
    /// <param name="registerDto">Données d'inscription</param>
    /// <returns>Token JWT et informations utilisateur</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.RegisterAsync(registerDto, ipAddress);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Inscription réussie"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Connexion d'un utilisateur
    /// </summary>
    /// <param name="loginDto">Identifiants</param>
    /// <returns>Token JWT et informations utilisateur</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.LoginAsync(loginDto, ipAddress);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Connexion réussie"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Rafraîchir le token JWT
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>Nouveau token JWT</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Authentification externe (Google, Facebook, Instagram)
    /// </summary>
    /// <param name="externalAuthDto">Token du provider externe</param>
    /// <returns>Token JWT et informations utilisateur</returns>
    [HttpPost("external")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExternalAuth([FromBody] ExternalAuthDto externalAuthDto)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.ExternalAuthAsync(externalAuthDto, ipAddress);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Authentification réussie"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during external authentication");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Confirmer l'email
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="token">Token de confirmation</param>
    /// <returns>Confirmation du statut</returns>
    [HttpGet("confirm-email")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        try
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Email confirmé avec succès"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Déconnexion - Révoque le refresh token
    /// </summary>
    /// <param name="request">Refresh token à révoquer</param>
    /// <returns>Confirmation de révocation</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto request)
    {
        try
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress);
            
            if (result)
            {
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Déconnexion réussie"));
            }
            
            return BadRequest(ApiResponse<string>.ErrorResponse("Token invalide"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Test endpoint pour vérifier l'authentification
    /// </summary>
    /// <returns>Informations utilisateur connecté</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        
        return Ok(ApiResponse<object>.SuccessResponse(new { UserId = userId, Email = email }));
    }
}
