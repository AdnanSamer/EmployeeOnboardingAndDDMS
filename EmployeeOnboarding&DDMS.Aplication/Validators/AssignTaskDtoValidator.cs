using FluentValidation;
using EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks;

namespace EmployeeOnboarding_DDMS.Aplication.Validators
{
    public class AssignTaskDtoValidator : AbstractValidator<AssignTaskDto>
    {
        public AssignTaskDtoValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Employee ID is required.");

            RuleFor(x => x.TaskTemplateId)
                .GreaterThan(0).WithMessage("Task template ID is required.");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");

            RuleFor(x => x.AssignedBy)
                .GreaterThan(0).WithMessage("Assigned by user ID is required.");

            RuleFor(x => x.Notes)
                .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.");
        }
    }
}

