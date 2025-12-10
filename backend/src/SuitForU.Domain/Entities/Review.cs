using SuitForU.Domain.Common;

namespace SuitForU.Domain.Entities;

public class Review : BaseEntity
{
    public Guid RentalId { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid ReviewedUserId { get; set; }
    public Guid GarmentId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    
    // Navigation properties
    public Rental Rental { get; set; } = null!;
    public User Reviewer { get; set; } = null!;
    public User ReviewedUser { get; set; } = null!;
    public Garment Garment { get; set; } = null!;
}
