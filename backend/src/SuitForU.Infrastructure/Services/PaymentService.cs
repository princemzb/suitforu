using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using SuitForU.Application.DTOs.Payments;
using SuitForU.Application.Interfaces;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Enums;
using SuitForU.Domain.Interfaces;

namespace SuitForU.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentService> _logger;
    private readonly IConfiguration _configuration;

    public PaymentService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ILogger<PaymentService> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _configuration = configuration;
        
        // Configurer la clé API Stripe
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<PaymentIntentDto> CreatePaymentIntentAsync(CreatePaymentDto createDto, Guid userId, CancellationToken cancellationToken = default)
    {
        // Vérifier que la réservation existe
        var rental = await _unitOfWork.Rentals.GetByIdAsync(createDto.RentalId, cancellationToken);
        if (rental == null)
        {
            throw new KeyNotFoundException("Rental not found");
        }

        // Vérifier que l'utilisateur est le locataire
        if (rental.RenterId != userId)
        {
            throw new UnauthorizedAccessException("Only the renter can create payment for this rental");
        }

        // Vérifier que la réservation est dans le bon statut
        if (rental.Status != RentalStatus.OwnerAccepted)
        {
            throw new InvalidOperationException($"Cannot create payment for rental with status {rental.Status}");
        }

        // Vérifier qu'il n'y a pas déjà un paiement réussi
        var existingPayment = (await _unitOfWork.Payments.GetAllAsync(cancellationToken))
            .FirstOrDefault(p => p.RentalId == createDto.RentalId && p.Status == PaymentStatus.Succeeded);

        if (existingPayment != null)
        {
            throw new InvalidOperationException("Payment already exists for this rental");
        }

        // Calculer le montant total (prix location + caution)
        var amount = rental.TotalPrice + rental.DepositAmount;

        // Créer PaymentIntent avec Stripe
        var paymentIntentOptions = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // Stripe utilise les centimes
            Currency = "eur",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
            Metadata = new Dictionary<string, string>
            {
                { "rental_id", createDto.RentalId.ToString() },
                { "user_id", userId.ToString() }
            }
        };
        
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentOptions, cancellationToken: cancellationToken);

        // Créer l'enregistrement de paiement
        var payment = new Payment
        {
            RentalId = createDto.RentalId,
            UserId = userId,
            Type = PaymentType.Rental,
            Method = Domain.Enums.PaymentMethod.CreditCard,
            Amount = amount,
            Status = PaymentStatus.Pending,
            PaymentIntentId = paymentIntent.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment intent created for rental {RentalId}, PaymentIntent {PaymentIntentId}, amount: {Amount}", 
            createDto.RentalId, paymentIntent.Id, amount);

        return new PaymentIntentDto
        {
            PaymentIntentId = paymentIntent.Id,
            ClientSecret = paymentIntent.ClientSecret,
            Amount = amount,
            Currency = "eur",
            Status = paymentIntent.Status
        };
    }

    public async Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto processDto, CancellationToken cancellationToken = default)
    {
        // Récupérer le paiement par PaymentIntentId
        var payment = (await _unitOfWork.Payments.GetAllAsync(cancellationToken))
            .FirstOrDefault(p => p.PaymentIntentId == processDto.PaymentIntentId);

        if (payment == null)
        {
            throw new KeyNotFoundException("Payment not found");
        }

        // Vérifier le statut actuel
        if (payment.Status == PaymentStatus.Succeeded)
        {
            throw new InvalidOperationException("Payment already processed");
        }

        try
        {
            // Vérifier le statut du PaymentIntent avec Stripe
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(processDto.PaymentIntentId, cancellationToken: cancellationToken);
            
            if (paymentIntent.Status != "succeeded")
            {
                throw new InvalidOperationException($"Payment not succeeded. Status: {paymentIntent.Status}");
            }

            payment.Status = PaymentStatus.Succeeded;
            payment.TransactionId = paymentIntent.Id;
            payment.StripeChargeId = paymentIntent.LatestChargeId;
            payment.ProcessedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment processed successfully for rental {RentalId}, Charge {ChargeId}", 
                payment.RentalId, payment.StripeChargeId);

            return _mapper.Map<PaymentDto>(payment);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error processing payment {PaymentIntentId}: {Message}", 
                processDto.PaymentIntentId, ex.Message);
            
            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = $"Stripe error: {ex.StripeError?.Message ?? ex.Message}";
            payment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment {PaymentIntentId}", processDto.PaymentIntentId);
            
            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = ex.Message;
            payment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw;
        }
    }

    public async Task<List<PaymentDto>> GetUserPaymentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var payments = (await _unitOfWork.Payments.GetAllAsync(cancellationToken))
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return _mapper.Map<List<PaymentDto>>(payments);
    }

    public async Task<PaymentDto> RefundPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId, cancellationToken);
        
        if (payment == null)
        {
            throw new KeyNotFoundException("Payment not found");
        }

        // Vérifier que le paiement a été effectué
        if (payment.Status != PaymentStatus.Succeeded)
        {
            throw new InvalidOperationException("Cannot refund payment that was not succeeded");
        }

        // Vérifier que le paiement n'a pas déjà été remboursé
        if (payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.PartiallyRefunded)
        {
            throw new InvalidOperationException("Payment already refunded");
        }

        try
        {
            // Créer un remboursement avec Stripe
            var refundOptions = new RefundCreateOptions
            {
                Charge = payment.StripeChargeId,
                Amount = (long)(payment.Amount * 100), // Remboursement complet en centimes
                Reason = RefundReasons.RequestedByCustomer
            };
            
            var refundService = new RefundService();
            var refund = await refundService.CreateAsync(refundOptions, cancellationToken: cancellationToken);

            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment {PaymentId} refunded successfully via Stripe Refund {RefundId}", 
                paymentId, refund.Id);

            return _mapper.Map<PaymentDto>(payment);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error refunding payment {PaymentId}: {Message}", 
                paymentId, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment {PaymentId}", paymentId);
            throw;
        }
    }
}
