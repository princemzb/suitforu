using SuitForU.Domain.Entities;
using SuitForU.Domain.Enums;

namespace SuitForU.Domain.Interfaces;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetByRenterIdAsync(Guid renterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rental>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rental>> GetByGarmentIdAsync(Guid garmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rental>> GetByStatusAsync(RentalStatus status, CancellationToken cancellationToken = default);
    Task<bool> IsGarmentAvailableAsync(Guid garmentId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<bool> HasActiveRentalsAsync(Guid garmentId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveOrFutureRentalsAsync(Guid garmentId, CancellationToken cancellationToken = default);
}
