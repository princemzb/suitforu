using SuitForU.Domain.Entities;
using SuitForU.Domain.Enums;

namespace SuitForU.Domain.Interfaces;

public interface IGarmentRepository : IRepository<Garment>
{
    Task<IEnumerable<Garment>> GetAvailableGarmentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Garment>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Garment>> SearchGarmentsAsync(
        string? searchTerm = null,
        string? city = null,
        decimal? maxPrice = null,
        GarmentType? type = null,
        string? size = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        string? searchTerm = null,
        string? city = null,
        decimal? maxPrice = null,
        GarmentType? type = null,
        string? size = null,
        CancellationToken cancellationToken = default);
    Task<Garment?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Garment?> GetGarmentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
