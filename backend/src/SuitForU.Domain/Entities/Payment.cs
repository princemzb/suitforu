using SuitForU.Domain.Common;
using SuitForU.Domain.Enums;

namespace SuitForU.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid RentalId { get; set; }
    public Guid UserId { get; set; }
    public PaymentType Type { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? StripeChargeId { get; set; }
    public string? PayPalOrderId { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
    
    // Navigation properties
    public Rental Rental { get; set; } = null!;
    public User User { get; set; } = null!;
}
