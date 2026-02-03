namespace CleanMe.Application.DTOs
{
    public class RegionIndexRowDTO
    {
        public int regionId { get; set; }
        public string RegionName { get; set; } = "";
        public string ReportCode { get; set; } = "";
        public bool IsActive { get; set; } = true;

        public int TotalCount { get; set; }   // returned by SP
    }
}
