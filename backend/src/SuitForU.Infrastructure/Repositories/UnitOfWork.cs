using Microsoft.EntityFrameworkCore.Storage;
using SuitForU.Domain.Interfaces;
using SuitForU.Infrastructure.Persistence;

namespace SuitForU.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IUserRepository? _users;
    private IGarmentRepository? _garments;
    private IRentalRepository? _rentals;
    private IPaymentRepository? _payments;
    private IRepository<Domain.Entities.Review>? _reviews;
    private IRepository<Domain.Entities.GarmentImage>? _garmentImages;
    private IRefreshTokenRepository? _refreshTokens;

    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository users,
        IGarmentRepository garments,
        IRentalRepository rentals,
        IPaymentRepository payments)
    {
        _context = context;
        _users = users;
        _garments = garments;
        _rentals = rentals;
        _payments = payments;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IGarmentRepository Garments => _garments ??= new GarmentRepository(_context);
    public IRentalRepository Rentals => _rentals ??= new RentalRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public IRepository<Domain.Entities.Review> Reviews => _reviews ??= new Repository<Domain.Entities.Review>(_context);
    public IRepository<Domain.Entities.GarmentImage> GarmentImages => _garmentImages ??= new Repository<Domain.Entities.GarmentImage>(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
