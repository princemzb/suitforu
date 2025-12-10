using SuitForU.Domain.Enums;

namespace SuitForU.Application.DTOs.Payments;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid RentalId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreatePaymentDto
{
    public Guid RentalId { get; set; }
    public PaymentType Type { get; set; }
    public PaymentMethod Method { get; set; }
    public string? PaymentToken { get; set; }
}

public class PaymentIntentDto
{
    public string ClientSecret { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }
}

public class ProcessPaymentDto
{
    public Guid PaymentId { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
}
