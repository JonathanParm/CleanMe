namespace CleanMe.Application.DTOs
{
    public class AssetIndexRowDTO
    {
        public int assetId { get; set; }
        public string AssetName { get; set; } = "";
        public string RegionName { get; set; } = "";
        public string AreaName { get; set; } = "";
        public string? MdReference { get; set; }
        public string ClientName { get; set; } = "";
        public string? ClientReference { get; set; }
        public string AssetLocation { get; set; } = "";
        public string ItemCode { get; set; } = "";

        public int TotalCount { get; set; }   // returned by SP
    }
}
