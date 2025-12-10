using SuitForU.Application.DTOs.Payments;

namespace SuitForU.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentDto> CreatePaymentIntentAsync(CreatePaymentDto createDto, Guid userId, CancellationToken cancellationToken = default);
    Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto processDto, CancellationToken cancellationToken = default);
    Task<List<PaymentDto>> GetUserPaymentsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PaymentDto> RefundPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
}
