using FluentValidation;
using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Validations;

public class ScheduleValidator : AbstractValidator<ScheduleViewModel>
{
    public ScheduleValidator()
    {
        RuleFor(x => x.CropId)
            .GreaterThan(0).WithMessage("Please select a crop.");

        RuleFor(x => x.FieldId)
            .GreaterThan(0).WithMessage("Please select a field.");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("Scheduled date is required.")
            .GreaterThan(DateTime.Today)
            .WithMessage("Scheduled date must be in the future.");

        RuleFor(x => x.ExpectedYieldKg)
            .GreaterThan(0).WithMessage("Expected yield must be greater than 0.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
    }
}