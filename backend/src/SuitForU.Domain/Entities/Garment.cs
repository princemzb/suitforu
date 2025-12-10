using SuitForU.Domain.Common;
using SuitForU.Domain.Enums;

namespace SuitForU.Domain.Entities;

public class Garment : BaseEntity
{
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public GarmentType Type { get; set; }
    public GarmentCondition Condition { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal DailyPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public string PickupAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int ViewCount { get; set; }
    
    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<GarmentImage> Images { get; set; } = new List<GarmentImage>();
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
