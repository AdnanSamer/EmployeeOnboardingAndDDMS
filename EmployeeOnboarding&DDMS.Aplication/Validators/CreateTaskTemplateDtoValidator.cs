using FluentValidation;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;

namespace EmployeeOnboarding_DDMS.Aplication.Validators
{
    public class CreateTaskTemplateDtoValidator : AbstractValidator<CreateTaskTemplateDto>
    {
        public CreateTaskTemplateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Task template name is required.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters.");

            RuleFor(x => x.EstimatedDays)
                .GreaterThan(0).When(x => x.EstimatedDays.HasValue)
                .WithMessage("Estimated days must be greater than 0.");
        }
    }
}

