using SuitForU.Domain.Entities;

namespace SuitForU.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByRentalIdAsync(Guid rentalId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
}
