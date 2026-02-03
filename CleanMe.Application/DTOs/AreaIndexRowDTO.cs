namespace CleanMe.Application.DTOs
{
    public class AreaIndexRowDTO
    {
        public int areaId { get; set; }
        public string RegionName { get; set; } = "";
        public string AreaName { get; set; } = "";
        public int ReportCode { get; set; } = 0;
        public string CleanerName { get; set; } = "";
        public bool IsActive { get; set; } = true;

        public int TotalCount { get; set; }   // returned by SP
    }
}
