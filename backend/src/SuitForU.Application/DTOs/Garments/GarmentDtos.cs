using SuitForU.Domain.Enums;

namespace SuitForU.Application.DTOs.Garments;

public class GarmentDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal DailyPrice { get; set; }
    public decimal DepositAmount { get; set; }
    public string City { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public List<GarmentImageDto> Images { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class GarmentDetailsDto : GarmentDto
{
    public string PickupAddress { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}

public class CreateGarmentDto
{
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
}

public class UpdateGarmentDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? DailyPrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public bool? IsAvailable { get; set; }
}

public class GarmentImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPrimary { get; set; }
}

public class GarmentSearchDto
{
    public string? SearchTerm { get; set; }
    public string? City { get; set; }
    public decimal? MaxPrice { get; set; }
    public GarmentType? Type { get; set; }
    public string? Size { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
