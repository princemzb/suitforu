using SuitForU.Application.DTOs;

namespace SuitForU.Application.Services;

/// <summary>
/// Service de gestion de la disponibilité des vêtements
/// </summary>
public interface IAvailabilityService
{
    /// <summary>
    /// Récupère le calendrier de disponibilité d'un vêtement (3 mois par défaut)
    /// </summary>
    Task<AvailabilityCalendarDto> GetAvailabilityCalendarAsync(Guid garmentId, int months = 3);
    
    /// <summary>
    /// Bloque manuellement des dates (propriétaire uniquement)
    /// </summary>
    Task BlockDatesAsync(Guid garmentId, BlockDatesDto dto, Guid ownerId);
    
    /// <summary>
    /// Débloque manuellement des dates (propriétaire uniquement)
    /// </summary>
    Task UnblockDatesAsync(Guid garmentId, UnblockDatesDto dto, Guid ownerId);
    
    /// <summary>
    /// Vérifie si une période est disponible
    /// </summary>
    Task<CheckAvailabilityDto> CheckAvailabilityAsync(Guid garmentId, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Bloque automatiquement les dates lors de la confirmation d'une location
    /// </summary>
    Task BlockDatesForRentalAsync(Guid garmentId, Guid rentalId, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Libère automatiquement les dates lors de l'annulation d'une location
    /// </summary>
    Task UnblockDatesForRentalAsync(Guid rentalId);
}
