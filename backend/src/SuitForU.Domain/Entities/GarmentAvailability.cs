using SuitForU.Domain.Common;

namespace SuitForU.Domain.Entities;

/// <summary>
/// Représente la disponibilité d'un vêtement pour une date spécifique
/// </summary>
public class GarmentAvailability : BaseEntity
{
    /// <summary>
    /// ID du vêtement concerné
    /// </summary>
    public Guid GarmentId { get; set; }
    
    /// <summary>
    /// Navigation vers le vêtement
    /// </summary>
    public Garment Garment { get; set; } = null!;
    
    /// <summary>
    /// Date concernée (jour spécifique)
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Indique si le vêtement est disponible ce jour
    /// </summary>
    public bool IsAvailable { get; set; }
    
    /// <summary>
    /// Raison du blocage
    /// </summary>
    public AvailabilityBlockReason? BlockedReason { get; set; }
    
    /// <summary>
    /// ID de la location si bloqué automatiquement
    /// </summary>
    public Guid? RentalId { get; set; }
    
    /// <summary>
    /// Navigation vers la location
    /// </summary>
    public Rental? Rental { get; set; }
    
    /// <summary>
    /// Notes du propriétaire sur le blocage
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Raisons possibles de blocage d'une date
/// </summary>
public enum AvailabilityBlockReason
{
    /// <summary>
    /// Bloqué par le propriétaire manuellement
    /// </summary>
    OwnerBlocked = 0,
    
    /// <summary>
    /// Bloqué automatiquement par une location confirmée
    /// </summary>
    Rental = 1,
    
    /// <summary>
    /// En maintenance ou nettoyage
    /// </summary>
    Maintenance = 2
}
