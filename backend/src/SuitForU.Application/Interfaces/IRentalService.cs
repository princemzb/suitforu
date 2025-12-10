using SuitForU.Application.DTOs.Rentals;

namespace SuitForU.Application.Interfaces;

public interface IRentalService
{
    Task<RentalDto> CreateRentalAsync(Guid renterId, CreateRentalDto createDto, CancellationToken cancellationToken = default);
    Task<RentalDetailsDto?> GetRentalByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<List<RentalDto>> GetUserRentalsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<RentalDto>> GetOwnerRentalsAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<RentalDto> AcceptRentalAsync(Guid rentalId, Guid ownerId, CancellationToken cancellationToken = default);
    Task<RentalDto> ConfirmRentalAsync(Guid rentalId, Guid renterId, CancellationToken cancellationToken = default);
    Task<RentalDto> ExtendRentalAsync(Guid rentalId, Guid userId, ExtendRentalDto extendDto, CancellationToken cancellationToken = default);
    Task CancelRentalAsync(Guid rentalId, Guid userId, CancelRentalDto cancelDto, CancellationToken cancellationToken = default);
}
