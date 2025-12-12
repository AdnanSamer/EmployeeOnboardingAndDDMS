namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Admin
{
    public class ActivityLogFilterDto
    {
        public int? UserId { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

