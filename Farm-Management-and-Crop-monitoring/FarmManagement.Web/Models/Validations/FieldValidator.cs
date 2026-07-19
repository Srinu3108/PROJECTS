using FluentValidation;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Validations;

public class FieldValidator : AbstractValidator<FieldViewModel>
{
    public FieldValidator()
    {
        RuleFor(x => x.FieldName)
            .NotEmpty().WithMessage("Field name is required.")
            .MaximumLength(100).WithMessage("Field name must not exceed 100 characters.");

        RuleFor(x => x.AreaHectares)
            .GreaterThan(0).WithMessage("Area must be greater than 0.");

        RuleFor(x => x.SoilType)
            .NotEmpty().WithMessage("Soil type is required.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.");
    }
}