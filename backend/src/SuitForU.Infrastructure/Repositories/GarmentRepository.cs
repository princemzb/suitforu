using Microsoft.EntityFrameworkCore;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Interfaces;
using SuitForU.Infrastructure.Persistence;

namespace SuitForU.Infrastructure.Repositories;

public class GarmentRepository : Repository<Garment>, IGarmentRepository
{
    public GarmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Garment>> GetAvailableGarmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.Owner)
            .Include(g => g.Images)
            .Where(g => g.IsAvailable && !g.IsDeleted)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Garment>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.Images)
            .Where(g => g.OwnerId == ownerId && !g.IsDeleted)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Garment>> SearchGarmentsAsync(
        string? searchTerm = null,
        string? city = null,
        decimal? maxPrice = null,
        Domain.Enums.GarmentType? type = null,
        string? size = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(g => g.Owner)
            .Include(g => g.Images)
            .Where(g => g.IsAvailable && !g.IsDeleted);

        // Appliquer les filtres
        query = ApplyFilters(query, searchTerm, city, maxPrice, type, size);

        return await query
            .OrderByDescending(g => g.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? searchTerm = null,
        string? city = null,
        decimal? maxPrice = null,
        Domain.Enums.GarmentType? type = null,
        string? size = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(g => g.IsAvailable && !g.IsDeleted);

        query = ApplyFilters(query, searchTerm, city, maxPrice, type, size);

        return await query.CountAsync(cancellationToken);
    }

    private IQueryable<Garment> ApplyFilters(
        IQueryable<Garment> query,
        string? searchTerm,
        string? city,
        decimal? maxPrice,
        Domain.Enums.GarmentType? type,
        string? size)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(g =>
                g.Title.Contains(searchTerm) ||
                g.Description.Contains(searchTerm) ||
                g.Brand.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(g => g.City.Contains(city));
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(g => g.DailyPrice <= maxPrice.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(g => g.Type == type.Value);
        }

        if (!string.IsNullOrWhiteSpace(size))
        {
            query = query.Where(g => g.Size == size);
        }

        return query;
    }

    public async Task<Garment?> GetGarmentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.Owner)
            .Include(g => g.Images)
            .Include(g => g.Reviews)
                .ThenInclude(r => r.Reviewer)
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
    }

    public async Task<Garment?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.Owner)
            .Include(g => g.Images)
            .Include(g => g.Reviews)
                .ThenInclude(r => r.Reviewer)
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
    }
}
