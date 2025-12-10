using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuitForU.Application.DTOs.Common;
using SuitForU.Application.DTOs.Payments;
using SuitForU.Application.Interfaces;
using System.Security.Claims;

namespace SuitForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Créer un PaymentIntent pour une réservation
    /// </summary>
    /// <param name="createDto">ID de la réservation</param>
    /// <returns>PaymentIntent avec client secret</returns>
    [HttpPost("create-intent")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PaymentIntentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var paymentIntent = await _paymentService.CreatePaymentIntentAsync(createDto, userId.Value);
            
            return Ok(ApiResponse<PaymentIntentDto>.SuccessResponse(
                paymentIntent, 
                "Payment intent created successfully"));
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
            _logger.LogError(ex, "Error creating payment intent");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Confirmer le paiement après succès côté client
    /// </summary>
    /// <param name="processDto">PaymentIntentId à confirmer</param>
    /// <returns>Paiement confirmé</returns>
    [HttpPost("confirm")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmPayment([FromBody] ProcessPaymentDto processDto)
    {
        try
        {
            var payment = await _paymentService.ProcessPaymentAsync(processDto);
            
            return Ok(ApiResponse<PaymentDto>.SuccessResponse(
                payment, 
                "Payment confirmed successfully"));
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
            _logger.LogError(ex, "Error confirming payment");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Récupérer l'historique des paiements de l'utilisateur
    /// </summary>
    /// <returns>Liste des paiements</returns>
    [HttpGet("my-payments")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<PaymentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyPayments()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<string>.ErrorResponse("User not authenticated"));
            }

            var payments = await _paymentService.GetUserPaymentsAsync(userId.Value);
            return Ok(ApiResponse<List<PaymentDto>>.SuccessResponse(payments));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user payments");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Rembourser un paiement
    /// </summary>
    /// <param name="id">ID du paiement</param>
    /// <returns>Paiement remboursé</returns>
    [HttpPost("{id:guid}/refund")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RefundPayment(Guid id)
    {
        try
        {
            var payment = await _paymentService.RefundPaymentAsync(id);
            
            return Ok(ApiResponse<PaymentDto>.SuccessResponse(
                payment, 
                "Payment refunded successfully"));
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
            _logger.LogError(ex, "Error refunding payment");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Webhook Stripe pour les événements de paiement
    /// </summary>
    /// <returns>Confirmation réception</returns>
    [HttpPost("webhook")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StripeWebhook()
    {
        try
        {
            // TODO: Implémenter la logique webhook Stripe
            // var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            // var stripeEvent = EventUtility.ConstructEvent(
            //     json,
            //     Request.Headers["Stripe-Signature"],
            //     _configuration["Stripe:WebhookSecret"]
            // );
            
            // switch (stripeEvent.Type)
            // {
            //     case "payment_intent.succeeded":
            //         var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            //         // Mettre à jour le paiement en base
            //         break;
            //     case "payment_intent.payment_failed":
            //         // Gérer l'échec
            //         break;
            // }

            _logger.LogInformation("Stripe webhook received");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Stripe webhook");
            return BadRequest();
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
