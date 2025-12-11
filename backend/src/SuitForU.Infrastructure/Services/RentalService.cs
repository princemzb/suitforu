using AutoMapper;
using SuitForU.Application.DTOs.Rentals;
using SuitForU.Application.Interfaces;
using SuitForU.Application.Services;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Enums;
using SuitForU.Domain.Interfaces;

namespace SuitForU.Infrastructure.Services;

public class RentalService : IRentalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAvailabilityService _availabilityService;

    public RentalService(IUnitOfWork unitOfWork, IMapper mapper, IAvailabilityService availabilityService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _availabilityService = availabilityService;
    }

    public async Task<RentalDto> CreateRentalAsync(Guid renterId, CreateRentalDto createDto, CancellationToken cancellationToken = default)
    {
        // Vérifier que le vêtement existe et est disponible
        var garment = await _unitOfWork.Garments.GetByIdAsync(createDto.GarmentId, cancellationToken);
        if (garment == null)
        {
            throw new KeyNotFoundException("Garment not found");
        }

        if (!garment.IsAvailable)
        {
            throw new InvalidOperationException("Garment is not available");
        }

        if (garment.IsDeleted)
        {
            throw new InvalidOperationException("Garment is no longer available");
        }

        // Vérifier que le locataire n'est pas le propriétaire
        if (garment.OwnerId == renterId)
        {
            throw new InvalidOperationException("You cannot rent your own garment");
        }

        // Vérifier les dates
        if (createDto.StartDate < DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("Start date cannot be in the past");
        }

        if (createDto.EndDate <= createDto.StartDate)
        {
            throw new InvalidOperationException("End date must be after start date");
        }

        // Vérifier la disponibilité du vêtement pour ces dates
        var isAvailable = await _unitOfWork.Rentals.IsGarmentAvailableAsync(
            createDto.GarmentId, 
            createDto.StartDate, 
            createDto.EndDate, 
            cancellationToken);

        if (!isAvailable)
        {
            throw new InvalidOperationException("Garment is already booked for these dates");
        }

        // Calculer la durée et le prix
        var durationDays = (createDto.EndDate - createDto.StartDate).Days + 1;
        var totalPrice = garment.DailyPrice * durationDays;

        // Créer la réservation
        var rental = new Rental
        {
            GarmentId = createDto.GarmentId,
            RenterId = renterId,
            OwnerId = garment.OwnerId,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            DurationDays = durationDays,
            DailyPrice = garment.DailyPrice,
            TotalPrice = totalPrice,
            DepositAmount = garment.DepositAmount,
            Status = RentalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Rentals.AddAsync(rental, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RentalDto>(rental);
    }

    public async Task<RentalDetailsDto?> GetRentalByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(id, cancellationToken);
        
        if (rental == null)
        {
            return null;
        }

        // Vérifier que l'utilisateur est concerné par cette réservation
        if (rental.RenterId != userId && rental.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to view this rental");
        }

        return _mapper.Map<RentalDetailsDto>(rental);
    }

    public async Task<List<RentalDto>> GetUserRentalsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rentals = (await _unitOfWork.Rentals.GetAllAsync(cancellationToken))
            .Where(r => r.RenterId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        return _mapper.Map<List<RentalDto>>(rentals);
    }

    public async Task<List<RentalDto>> GetOwnerRentalsAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var rentals = (await _unitOfWork.Rentals.GetAllAsync(cancellationToken))
            .Where(r => r.OwnerId == ownerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        return _mapper.Map<List<RentalDto>>(rentals);
    }

    public async Task<RentalDto> AcceptRentalAsync(Guid rentalId, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId, cancellationToken);
        
        if (rental == null)
        {
            throw new KeyNotFoundException("Rental not found");
        }

        // Vérifier que l'utilisateur est le propriétaire
        if (rental.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("Only the owner can accept this rental");
        }

        // Vérifier le statut
        if (rental.Status != RentalStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot accept rental with status {rental.Status}");
        }

        // Vérifier que le vêtement est toujours disponible
        var garment = await _unitOfWork.Garments.GetByIdAsync(rental.GarmentId, cancellationToken);
        if (garment == null || !garment.IsAvailable || garment.IsDeleted)
        {
            throw new InvalidOperationException("Garment is no longer available");
        }

        // Accepter la réservation
        rental.Status = RentalStatus.OwnerAccepted;
        rental.OwnerAcceptedAt = DateTime.UtcNow;
        rental.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Rentals.UpdateAsync(rental, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RentalDto>(rental);
    }

    public async Task<RentalDto> ConfirmRentalAsync(Guid rentalId, Guid renterId, CancellationToken cancellationToken = default)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId, cancellationToken);
        
        if (rental == null)
        {
            throw new KeyNotFoundException("Rental not found");
        }

        // Vérifier que l'utilisateur est le locataire
        if (rental.RenterId != renterId)
        {
            throw new UnauthorizedAccessException("Only the renter can confirm this rental");
        }

        // Vérifier le statut
        if (rental.Status != RentalStatus.OwnerAccepted)
        {
            throw new InvalidOperationException($"Cannot confirm rental with status {rental.Status}. Owner must accept first.");
        }

        // Vérifier qu'un paiement a été effectué
        var hasPayment = (await _unitOfWork.Payments.GetAllAsync(cancellationToken))
            .Any(p => p.RentalId == rentalId && p.Status == PaymentStatus.Succeeded);

        if (!hasPayment)
        {
            throw new InvalidOperationException("Payment must be completed before confirming rental");
        }

        // Confirmer la réservation
        rental.Status = RentalStatus.Confirmed;
        rental.RenterConfirmedAt = DateTime.UtcNow;
        rental.UpdatedAt = DateTime.UtcNow;

        // Marquer le vêtement comme non disponible
        var garment = await _unitOfWork.Garments.GetByIdAsync(rental.GarmentId, cancellationToken);
        if (garment != null)
        {
            garment.IsAvailable = false;
            garment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Garments.UpdateAsync(garment, cancellationToken);
        }

        await _unitOfWork.Rentals.UpdateAsync(rental, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Bloquer automatiquement les dates dans le calendrier de disponibilité
        await _availabilityService.BlockDatesForRentalAsync(rental.GarmentId, rental.Id, rental.StartDate, rental.EndDate);

        return _mapper.Map<RentalDto>(rental);
    }

    public async Task<RentalDto> ExtendRentalAsync(Guid rentalId, Guid userId, ExtendRentalDto extendDto, CancellationToken cancellationToken = default)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId, cancellationToken);
        
        if (rental == null)
        {
            throw new KeyNotFoundException("Rental not found");
        }

        // Vérifier que l'utilisateur est concerné
        if (rental.RenterId != userId && rental.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to extend this rental");
        }

        // Vérifier le statut (seulement Active ou Confirmed)
        if (rental.Status != RentalStatus.Active && rental.Status != RentalStatus.Confirmed)
        {
            throw new InvalidOperationException($"Cannot extend rental with status {rental.Status}");
        }

        // Vérifier que la nouvelle date est après la date actuelle
        if (extendDto.NewEndDate <= rental.EndDate)
        {
            throw new InvalidOperationException("New end date must be after current end date");
        }

        // Vérifier la disponibilité du vêtement pour les dates étendues
        var isAvailable = await _unitOfWork.Rentals.IsGarmentAvailableAsync(
            rental.GarmentId,
            rental.EndDate.AddDays(1),
            extendDto.NewEndDate,
            cancellationToken);

        if (!isAvailable)
        {
            throw new InvalidOperationException("Garment is already booked for the extended period");
        }

        // Calculer les jours supplémentaires et le coût additionnel
        var additionalDays = (extendDto.NewEndDate - rental.EndDate).Days;
        var additionalPrice = rental.DailyPrice * additionalDays;

        // Mettre à jour la réservation
        rental.EndDate = extendDto.NewEndDate;
        rental.DurationDays += additionalDays;
        rental.TotalPrice += additionalPrice;
        rental.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Rentals.UpdateAsync(rental, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RentalDto>(rental);
    }

    public async Task CancelRentalAsync(Guid rentalId, Guid userId, CancelRentalDto cancelDto, CancellationToken cancellationToken = default)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId, cancellationToken);
        
        if (rental == null)
        {
            throw new KeyNotFoundException("Rental not found");
        }

        // Vérifier que l'utilisateur est concerné
        if (rental.RenterId != userId && rental.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to cancel this rental");
        }

        // Vérifier le statut (ne peut pas annuler si déjà complété ou annulé)
        if (rental.Status == RentalStatus.Completed || rental.Status == RentalStatus.Cancelled)
        {
            throw new InvalidOperationException($"Cannot cancel rental with status {rental.Status}");
        }

        // Annuler la réservation
        rental.Status = RentalStatus.Cancelled;
        rental.CancellationReason = cancelDto.Reason;
        rental.CancelledAt = DateTime.UtcNow;
        rental.UpdatedAt = DateTime.UtcNow;

        // Remettre le vêtement comme disponible
        var garment = await _unitOfWork.Garments.GetByIdAsync(rental.GarmentId, cancellationToken);
        if (garment != null)
        {
            garment.IsAvailable = true;
            garment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Garments.UpdateAsync(garment, cancellationToken);
        }

        await _unitOfWork.Rentals.UpdateAsync(rental, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Libérer automatiquement les dates dans le calendrier de disponibilité
        await _availabilityService.UnblockDatesForRentalAsync(rental.Id);
    }
}
