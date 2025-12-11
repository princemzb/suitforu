namespace SuitForU.Application.DTOs;

/// <summary>
/// DTO pour la disponibilité d'un vêtement sur une date
/// </summary>
public class AvailabilityDto
{
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
    public string? BlockedReason { get; set; }
    public Guid? RentalId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO pour le calendrier de disponibilité (vue complète 3 mois)
/// </summary>
public class AvailabilityCalendarDto
{
    public Guid GarmentId { get; set; }
    public string GarmentTitle { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<AvailabilityDto> Availabilities { get; set; } = new();
}

/// <summary>
/// DTO pour bloquer des dates
/// </summary>
public class BlockDatesDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO pour débloquer des dates
/// </summary>
public class UnblockDatesDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

/// <summary>
/// DTO pour vérifier la disponibilité d'une période
/// </summary>
public class CheckAvailabilityDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAvailable { get; set; }
    public List<DateTime> UnavailableDates { get; set; } = new();
}
