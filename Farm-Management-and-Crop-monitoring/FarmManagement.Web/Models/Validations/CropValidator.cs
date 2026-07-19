using FluentValidation;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Validations;

public class CropValidator : AbstractValidator<CropViewModel>
{
    public CropValidator()
    {
        RuleFor(x => x.CropName)
            .NotEmpty().WithMessage("Crop name is required.")
            .MaximumLength(100).WithMessage("Crop name must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s\-]+$").WithMessage("Crop name can only contain letters, spaces and hyphens.");

        RuleFor(x => x.CropType)
            .NotEmpty().WithMessage("Crop type is required.");

        RuleFor(x => x.FieldId)
            .GreaterThan(0).WithMessage("Please select a valid field.");

        RuleFor(x => x.PlantingDate)
            .NotEmpty().WithMessage("Planting date is required.")
            .LessThan(x => x.ExpectedHarvestDate)
            .WithMessage("Planting date must be before expected harvest date.");

        RuleFor(x => x.ExpectedHarvestDate)
            .NotEmpty().WithMessage("Expected harvest date is required.")
            .GreaterThan(DateTime.Today)
            .WithMessage("Expected harvest date must be in the future.");
    }
}