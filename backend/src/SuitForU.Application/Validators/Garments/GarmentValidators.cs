using FluentValidation;
using SuitForU.Application.DTOs.Garments;

namespace SuitForU.Application.Validators.Garments;

public class CreateGarmentDtoValidator : AbstractValidator<CreateGarmentDto>
{
    public CreateGarmentDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Size)
            .NotEmpty().WithMessage("Size is required");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required")
            .MaximumLength(100).WithMessage("Brand must not exceed 100 characters");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required")
            .MaximumLength(50).WithMessage("Color must not exceed 50 characters");

        RuleFor(x => x.DailyPrice)
            .GreaterThan(0).WithMessage("Daily price must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Daily price must not exceed 10000");

        RuleFor(x => x.DepositAmount)
            .GreaterThan(0).WithMessage("Deposit amount must be greater than 0")
            .LessThanOrEqualTo(50000).WithMessage("Deposit amount must not exceed 50000");

        RuleFor(x => x.PickupAddress)
            .NotEmpty().WithMessage("Pickup address is required")
            .MaximumLength(500).WithMessage("Pickup address must not exceed 500 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters");
    }
}
