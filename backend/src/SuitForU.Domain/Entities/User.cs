using SuitForU.Domain.Common;
using SuitForU.Domain.Enums;

namespace SuitForU.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;
    public string? ExternalProviderId { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties
    public ICollection<Garment> Garments { get; set; } = new List<Garment>();
    public ICollection<Rental> RentalsAsRenter { get; set; } = new List<Rental>();
    public ICollection<Rental> RentalsAsOwner { get; set; } = new List<Rental>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
