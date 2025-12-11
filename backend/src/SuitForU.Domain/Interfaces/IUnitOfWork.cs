namespace SuitForU.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IGarmentRepository Garments { get; }
    IRentalRepository Rentals { get; }
    IPaymentRepository Payments { get; }
    IRepository<Domain.Entities.Review> Reviews { get; }
    IRepository<Domain.Entities.GarmentImage> GarmentImages { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IRepository<Domain.Entities.Conversation> Conversations { get; }
    IRepository<Domain.Entities.Message> Messages { get; }
    IRepository<Domain.Entities.GarmentAvailability> GarmentAvailabilities { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
