using FluentValidation;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Validations;

public class InventoryValidator : AbstractValidator<InventoryViewModel>
{
    public InventoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Resource name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Please select a valid resource type.");
    }
}
