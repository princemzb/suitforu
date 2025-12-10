using SuitForU.Domain.Entities;

namespace SuitForU.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByExternalProviderIdAsync(string providerId, CancellationToken cancellationToken = default);
    Task<User?> GetByExternalProviderAsync(Domain.Enums.AuthProvider provider, string externalId, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
