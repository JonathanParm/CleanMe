namespace CleanMe.Application.DTOs
{
    public class CleanFrequencyIndexRowDTO
    {
        public int cleanFrequencyId { get; set; }
        public string CleanFrequencyName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Code { get; set; } = "";
        public bool IsActive { get; set; } = true;

        public int TotalCount { get; set; }   // returned by SP
    }
}
