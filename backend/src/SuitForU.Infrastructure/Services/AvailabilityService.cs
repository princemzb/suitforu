using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SuitForU.Application.DTOs;
using SuitForU.Application.Services;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Interfaces;
using SuitForU.Infrastructure.Persistence;

namespace SuitForU.Infrastructure.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AvailabilityService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AvailabilityCalendarDto> GetAvailabilityCalendarAsync(Guid garmentId, int months = 3)
    {
        // Vérifier que le vêtement existe
        var garment = await _unitOfWork.Garments.GetByIdAsync(garmentId);
        if (garment == null || garment.IsDeleted)
            throw new KeyNotFoundException("Garment not found");

        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddMonths(months);

        // Récupérer toutes les disponibilités existantes pour cette période
        var allAvailabilities = await _unitOfWork.GarmentAvailabilities.GetAllAsync();
        var existingAvailabilities = allAvailabilities.Where(ga => ga.GarmentId == garmentId && 
                               ga.Date >= startDate && 
                               ga.Date <= endDate &&
                               !ga.IsDeleted);

        var availabilityDict = existingAvailabilities.ToDictionary(ga => ga.Date.Date, ga => ga);

        // Générer la liste complète des dates
        var availabilities = new List<AvailabilityDto>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (availabilityDict.TryGetValue(date, out var availability))
            {
                // Date avec disponibilité enregistrée
                availabilities.Add(new AvailabilityDto
                {
                    Date = date,
                    IsAvailable = availability.IsAvailable,
                    BlockedReason = availability.BlockedReason?.ToString(),
                    RentalId = availability.RentalId,
                    Notes = availability.Notes
                });
            }
            else
            {
                // Date disponible par défaut
                availabilities.Add(new AvailabilityDto
                {
                    Date = date,
                    IsAvailable = true,
                    BlockedReason = null,
                    RentalId = null,
                    Notes = null
                });
            }
        }

        return new AvailabilityCalendarDto
        {
            GarmentId = garmentId,
            GarmentTitle = garment.Title,
            StartDate = startDate,
            EndDate = endDate,
            Availabilities = availabilities
        };
    }

    public async Task BlockDatesAsync(Guid garmentId, BlockDatesDto dto, Guid ownerId)
    {
        // Vérifier que le vêtement existe et appartient à l'utilisateur
        var garment = await _unitOfWork.Garments.GetByIdAsync(garmentId);
        if (garment == null || garment.IsDeleted)
            throw new KeyNotFoundException("Garment not found");

        if (garment.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You are not the owner of this garment");

        // Valider les dates
        if (dto.StartDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Cannot block dates in the past");

        if (dto.EndDate < dto.StartDate)
            throw new ArgumentException("End date must be after start date");

        var maxDate = DateTime.UtcNow.Date.AddMonths(12);
        if (dto.EndDate > maxDate)
            throw new ArgumentException("Cannot block dates more than 12 months in advance");

        // Bloquer toutes les dates de la période
        for (var date = dto.StartDate.Date; date <= dto.EndDate.Date; date = date.AddDays(1))
        {
            var allAvailabilities = await _unitOfWork.GarmentAvailabilities.GetAllAsync();
            var existing = allAvailabilities.FirstOrDefault(ga => ga.GarmentId == garmentId && ga.Date == date && !ga.IsDeleted);

            if (existing != null)
            {
                // Mettre à jour si pas déjà bloqué par une location
                if (existing.BlockedReason != AvailabilityBlockReason.Rental)
                {
                    existing.IsAvailable = false;
                    existing.BlockedReason = AvailabilityBlockReason.OwnerBlocked;
                    existing.Notes = dto.Notes;
                }
            }
            else
            {
                // Créer un nouveau blocage
                var availability = new GarmentAvailability
                {
                    GarmentId = garmentId,
                    Date = date,
                    IsAvailable = false,
                    BlockedReason = AvailabilityBlockReason.OwnerBlocked,
                    Notes = dto.Notes
                };
                await _unitOfWork.GarmentAvailabilities.AddAsync(availability);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnblockDatesAsync(Guid garmentId, UnblockDatesDto dto, Guid ownerId)
    {
        // Vérifier que le vêtement existe et appartient à l'utilisateur
        var garment = await _unitOfWork.Garments.GetByIdAsync(garmentId);
        if (garment == null || garment.IsDeleted)
            throw new KeyNotFoundException("Garment not found");

        if (garment.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You are not the owner of this garment");

        // Débloquer les dates (seulement celles bloquées manuellement)
        var allAvailabilities = await _unitOfWork.GarmentAvailabilities.GetAllAsync();
        var availabilities = allAvailabilities.Where(ga => ga.GarmentId == garmentId &&
                               ga.Date >= dto.StartDate.Date &&
                               ga.Date <= dto.EndDate.Date &&
                               !ga.IsDeleted);

        foreach (var availability in availabilities)
        {
            // Ne débloquer que les dates bloquées manuellement par le propriétaire
            if (availability.BlockedReason == AvailabilityBlockReason.OwnerBlocked)
            {
                availability.IsAvailable = true;
                availability.BlockedReason = null;
                availability.Notes = null;
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<CheckAvailabilityDto> CheckAvailabilityAsync(Guid garmentId, DateTime startDate, DateTime endDate)
    {
        // Vérifier que le vêtement existe
        var garment = await _unitOfWork.Garments.GetByIdAsync(garmentId);
        if (garment == null || garment.IsDeleted)
            throw new KeyNotFoundException("Garment not found");

        var unavailableDates = new List<DateTime>();

        // Vérifier chaque date de la période
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var allAvailabilities = await _unitOfWork.GarmentAvailabilities.GetAllAsync();
            var availability = allAvailabilities.FirstOrDefault(ga => ga.GarmentId == garmentId && ga.Date == date && !ga.IsDeleted);

            if (availability != null && !availability.IsAvailable)
            {
                unavailableDates.Add(date);
            }
        }

        return new CheckAvailabilityDto
        {
            StartDate = startDate,
            EndDate = endDate,
            IsAvailable = unavailableDates.Count == 0,
            UnavailableDates = unavailableDates
        };
    }

    public async Task BlockDatesForRentalAsync(Guid garmentId, Guid rentalId, DateTime startDate, DateTime endDate)
    {
        // Bloquer automatiquement toutes les dates de la location
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var allAvailabilities = await _unitOfWork.GarmentAvailabilities.GetAllAsync();
            var existing = allAvailabilities.FirstOrDefault(ga => ga.GarmentId == garmentId && ga.Date == date && !ga.IsDeleted);

            if (existing != null)
            {
                existing.IsAvailable = false;
                existing.BlockedReason = AvailabilityBlockReason.Rental;
                existing.RentalId = rentalId;
            }
            else
            {
                var availability = new GarmentAvailability
                {
                    GarmentId = garmentId,
                    Date = date,
                    IsAvailable = false,
                    BlockedReason = AvailabilityBlockReason.Rental,
                    RentalId = rentalId
                };
                await _unitOfWork.GarmentAvailabilities.AddAsync(availability);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnblockDatesForRentalAsync(Guid rentalId)
    {
        // Libérer toutes les dates bloquées pour cette location
        var allAvailabilities = await _unitOfWork.GarmentAvailabilities.GetAllAsync();
        var availabilities = allAvailabilities.Where(ga => ga.RentalId == rentalId && !ga.IsDeleted);

        foreach (var availability in availabilities)
        {
            availability.IsAvailable = true;
            availability.BlockedReason = null;
            availability.RentalId = null;
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
