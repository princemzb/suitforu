using SuitForU.Domain.Common;

namespace SuitForU.Domain.Entities;

public class GarmentImage : BaseEntity
{
    public Guid GarmentId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPrimary { get; set; }
    
    // Navigation properties
    public Garment Garment { get; set; } = null!;
}
