using SuitForU.Domain.Common;
using SuitForU.Domain.Enums;

namespace SuitForU.Domain.Entities;

public class Rental : BaseEntity
{
    public Guid GarmentId { get; set; }
    public Guid RenterId { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public decimal DailyPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Pending;
    public DateTime? OwnerAcceptedAt { get; set; }
    public DateTime? RenterConfirmedAt { get; set; }
    public DateTime? PickupConfirmedAt { get; set; }
    public DateTime? ReturnConfirmedAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Navigation properties
    public Garment Garment { get; set; } = null!;
    public User Renter { get; set; } = null!;
    public User Owner { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Review? Review { get; set; }
}
