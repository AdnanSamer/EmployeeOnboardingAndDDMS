namespace EmployeeOnboarding_DDMS.Aplication.DTOs.Admin
{
    public class ChartDataDto
    {
        public IEnumerable<string> Labels { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<ChartDatasetDto> Datasets { get; set; } = Enumerable.Empty<ChartDatasetDto>();
    }

    public class ChartDatasetDto
    {
        public string Label { get; set; } = string.Empty;
        public IEnumerable<double> Data { get; set; } = Enumerable.Empty<double>();
        public IEnumerable<string>? BackgroundColor { get; set; }
        public IEnumerable<string>? BorderColor { get; set; }
    }
}

