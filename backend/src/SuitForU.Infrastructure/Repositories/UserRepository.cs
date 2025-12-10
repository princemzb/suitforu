using Microsoft.EntityFrameworkCore;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Interfaces;
using SuitForU.Infrastructure.Persistence;

namespace SuitForU.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
    }

    public async Task<User?> GetByExternalProviderIdAsync(string providerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.ExternalProviderId == providerId && !u.IsDeleted, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
    }

    public async Task<User?> GetByExternalProviderAsync(Domain.Enums.AuthProvider provider, string externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.AuthProvider == provider && u.ExternalProviderId == externalId && !u.IsDeleted, cancellationToken);
    }
}
