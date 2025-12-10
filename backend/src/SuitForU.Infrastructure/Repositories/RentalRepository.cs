using Microsoft.EntityFrameworkCore;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Enums;
using SuitForU.Domain.Interfaces;
using SuitForU.Infrastructure.Persistence;

namespace SuitForU.Infrastructure.Repositories;

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    public RentalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rental>> GetByRenterIdAsync(Guid renterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Garment)
                .ThenInclude(g => g.Images)
            .Include(r => r.Owner)
            .Where(r => r.RenterId == renterId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Garment)
                .ThenInclude(g => g.Images)
            .Include(r => r.Renter)
            .Where(r => r.OwnerId == ownerId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetByGarmentIdAsync(Guid garmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.GarmentId == garmentId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Garment)
            .Include(r => r.Renter)
            .Include(r => r.Owner)
            .Where(r => r.Status == status && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsGarmentAvailableAsync(Guid garmentId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return !await _dbSet.AnyAsync(r =>
            r.GarmentId == garmentId &&
            !r.IsDeleted &&
            r.Status != RentalStatus.Cancelled &&
            r.Status != RentalStatus.Completed &&
            (
                (startDate >= r.StartDate && startDate <= r.EndDate) ||
                (endDate >= r.StartDate && endDate <= r.EndDate) ||
                (startDate <= r.StartDate && endDate >= r.EndDate)
            ),
            cancellationToken);
    }

    public async Task<bool> HasActiveRentalsAsync(Guid garmentId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet.AnyAsync(r =>
            r.GarmentId == garmentId &&
            !r.IsDeleted &&
            (r.Status == RentalStatus.Active || r.Status == RentalStatus.Confirmed) &&
            r.StartDate <= today &&
            r.EndDate >= today,
            cancellationToken);
    }

    public async Task<bool> HasActiveOrFutureRentalsAsync(Guid garmentId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet.AnyAsync(r =>
            r.GarmentId == garmentId &&
            !r.IsDeleted &&
            (r.Status == RentalStatus.Active || 
             r.Status == RentalStatus.Confirmed || 
             r.Status == RentalStatus.OwnerAccepted) &&
            r.EndDate >= today,
            cancellationToken);
    }
}
