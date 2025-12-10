using AutoMapper;
using Microsoft.Extensions.Logging;
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
    // Note: Pour l'intégration Stripe réelle, ajouter: private readonly Stripe.StripeClient _stripeClient;

    public PaymentService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        ILogger<PaymentService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
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

        // TODO: Créer PaymentIntent avec Stripe
        // var paymentIntentOptions = new PaymentIntentCreateOptions
        // {
        //     Amount = (long)(amount * 100), // Stripe utilise les centimes
        //     Currency = "eur",
        //     Metadata = new Dictionary<string, string>
        //     {
        //         { "rental_id", createDto.RentalId.ToString() },
        //         { "user_id", userId.ToString() }
        //     }
        // };
        // var paymentIntent = await _stripeClient.PaymentIntents.CreateAsync(paymentIntentOptions);

        // Pour le MVP, on simule la création
        var simulatedPaymentIntentId = $"pi_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 24)}";
        var simulatedClientSecret = $"{simulatedPaymentIntentId}_secret_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16)}";

        // Créer l'enregistrement de paiement
        var payment = new Payment
        {
            RentalId = createDto.RentalId,
            UserId = userId,
            Type = PaymentType.Rental,
            Method = PaymentMethod.CreditCard, // Stripe = CreditCard par défaut
            Amount = amount,
            Status = PaymentStatus.Pending,
            PaymentIntentId = simulatedPaymentIntentId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment intent created for rental {RentalId}, amount: {Amount}", createDto.RentalId, amount);

        return new PaymentIntentDto
        {
            PaymentIntentId = simulatedPaymentIntentId,
            ClientSecret = simulatedClientSecret,
            Amount = amount,
            Currency = "eur",
            Status = "pending"
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
            // TODO: Vérifier le statut du PaymentIntent avec Stripe
            // var paymentIntent = await _stripeClient.PaymentIntents.GetAsync(processDto.PaymentIntentId);
            // if (paymentIntent.Status != "succeeded")
            // {
            //     throw new InvalidOperationException($"Payment not succeeded. Status: {paymentIntent.Status}");
            // }
            // payment.StripeChargeId = paymentIntent.LatestChargeId;

            // Pour le MVP, on simule le succès
            payment.Status = PaymentStatus.Succeeded;
            payment.TransactionId = $"txn_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 24)}";
            payment.StripeChargeId = $"ch_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 24)}";
            payment.ProcessedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment processed successfully for rental {RentalId}", payment.RentalId);

            return _mapper.Map<PaymentDto>(payment);
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
            // TODO: Créer un remboursement avec Stripe
            // var refundOptions = new RefundCreateOptions
            // {
            //     Charge = payment.StripeChargeId,
            //     Amount = (long)(payment.Amount * 100) // Remboursement complet
            // };
            // var refund = await _stripeClient.Refunds.CreateAsync(refundOptions);

            // Pour le MVP, on simule le remboursement
            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment {PaymentId} refunded successfully", paymentId);

            return _mapper.Map<PaymentDto>(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment {PaymentId}", paymentId);
            throw;
        }
    }
}
