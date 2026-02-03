namespace CleanMe.Application.DTOs
{
    public class AssetLocationIndexRowDTO
    {
        public int assetLocationId { get; set; }
        public string AreaName { get; set; } = "";
        public string Description { get; set; } = "";
        public string? ReportCode { get; set; }
        public string TownSuburb { get; set; } = "";
        public bool IsActive { get; set; } = true;

        public int TotalCount { get; set; }   // returned by SP
    }
}
