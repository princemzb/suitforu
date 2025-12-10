using AutoMapper;
using SuitForU.Application.DTOs.Auth;
using SuitForU.Application.DTOs.Garments;
using SuitForU.Application.DTOs.Rentals;
using SuitForU.Application.DTOs.Payments;
using SuitForU.Domain.Entities;

namespace SuitForU.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // Garment mappings
        CreateMap<Garment, GarmentDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.Owner.FirstName} {src.Owner.LastName}"))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Condition, opt => opt.MapFrom(src => src.Condition.ToString()));

        CreateMap<Garment, GarmentDetailsDto>()
            .IncludeBase<Garment, GarmentDto>()
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count));

        CreateMap<CreateGarmentDto, Garment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore());

        CreateMap<GarmentImage, GarmentImageDto>();

        // Rental mappings
        CreateMap<Rental, RentalDto>()
            .ForMember(dest => dest.GarmentTitle, opt => opt.MapFrom(src => src.Garment.Title))
            .ForMember(dest => dest.GarmentImageUrl, opt => opt.MapFrom(src => 
                src.Garment.Images.FirstOrDefault(i => i.IsPrimary) != null 
                    ? src.Garment.Images.First(i => i.IsPrimary).ImageUrl 
                    : src.Garment.Images.FirstOrDefault() != null ? src.Garment.Images.First().ImageUrl : ""))
            .ForMember(dest => dest.RenterName, opt => opt.MapFrom(src => $"{src.Renter.FirstName} {src.Renter.LastName}"))
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.Owner.FirstName} {src.Owner.LastName}"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Rental, RentalDetailsDto>()
            .IncludeBase<Rental, RentalDto>()
            .ForMember(dest => dest.PickupAddress, opt => opt.MapFrom(src => src.Garment.PickupAddress));

        CreateMap<CreateRentalDto, Rental>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => (src.EndDate - src.StartDate).Days));

        // Payment mappings
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreatePaymentDto, Payment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
