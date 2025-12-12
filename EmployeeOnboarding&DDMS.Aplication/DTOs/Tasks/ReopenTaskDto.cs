namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Tasks
{
    public class ReopenTaskDto
    {
        public string? Reason { get; set; }
        public DateTime? NewDueDate { get; set; }
        public int ReopenedBy { get; set; }
    }
}

