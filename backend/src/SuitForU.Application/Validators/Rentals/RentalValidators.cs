using FluentValidation;
using SuitForU.Application.DTOs.Rentals;

namespace SuitForU.Application.Validators.Rentals;

public class CreateRentalDtoValidator : AbstractValidator<CreateRentalDto>
{
    public CreateRentalDtoValidator()
    {
        RuleFor(x => x.GarmentId)
            .NotEmpty().WithMessage("Garment ID is required");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date must be today or in the future");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 30)
            .WithMessage("Rental duration cannot exceed 30 days");
    }
}
