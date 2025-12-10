using SuitForU.Domain.Enums;

namespace SuitForU.Application.DTOs.Rentals;

public class RentalDto
{
    public Guid Id { get; set; }
    public Guid GarmentId { get; set; }
    public string GarmentTitle { get; set; } = string.Empty;
    public string GarmentImageUrl { get; set; } = string.Empty;
    public Guid RenterId { get; set; }
    public string RenterName { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public decimal DailyPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateRentalDto
{
    public Guid GarmentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class RentalDetailsDto : RentalDto
{
    public string PickupAddress { get; set; } = string.Empty;
    public DateTime? OwnerAcceptedAt { get; set; }
    public DateTime? RenterConfirmedAt { get; set; }
    public DateTime? PickupConfirmedAt { get; set; }
    public DateTime? ReturnConfirmedAt { get; set; }
}

public class AcceptRentalDto
{
    public Guid RentalId { get; set; }
}

public class ExtendRentalDto
{
    public Guid RentalId { get; set; }
    public DateTime NewEndDate { get; set; }
}

public class CancelRentalDto
{
    public Guid RentalId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
