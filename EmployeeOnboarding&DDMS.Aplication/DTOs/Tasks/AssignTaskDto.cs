namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks
{
    public class AssignTaskDto
    {
        public int EmployeeId { get; set; }
        public int TaskTemplateId { get; set; }
        public DateTime DueDate { get; set; }
        public int AssignedBy { get; set; }
        public string? Notes { get; set; }
    }
}

