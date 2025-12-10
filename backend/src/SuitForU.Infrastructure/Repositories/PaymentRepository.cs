using Microsoft.EntityFrameworkCore;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Interfaces;
using SuitForU.Infrastructure.Persistence;

namespace SuitForU.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetByRentalIdAsync(Guid rentalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.RentalId == rentalId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Rental)
                .ThenInclude(r => r.Garment)
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId && !p.IsDeleted, cancellationToken);
    }
}
