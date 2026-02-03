namespace CleanMe.Application.DTOs
{
    public class AmendmentIndexRowDTO
    {
        public int amendmentId { get; set; }
        public string SourceName { get; set; } = "";
        public string AmendmentTypeName { get; set; } = "";
        public string ClientName { get; set; } = "";
        public string AreaName { get; set; } = "";
        public string LocationName { get; set; } = "";
        public string MdReference { get; set; } = "";
        public string ClientReference { get; set; } = "";
        public string AmendmentSummary { get; set; } = "";

        public int TotalCount { get; set; }   // returned by SP
    }
}
