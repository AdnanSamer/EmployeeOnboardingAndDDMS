using EmployeeOnboarding_DDMS.Domain.Enums;

namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks
{
    public class UpdateTaskStatusDto
    {
        public int TaskId { get; set; }
        public Domain.Enums.TaskStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}

