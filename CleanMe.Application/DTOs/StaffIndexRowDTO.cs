namespace CleanMe.Application.DTOs
{
    public class StaffIndexRowDTO
    {
        public int staffId { get; set; }
        public string FullName { get; set; } = "";
        public string WorkRole { get; set; } = "";
        public string ContactDetail { get; set; } = "";
        public bool IsActive { get; set; } = true;

        public int TotalCount { get; set; }   // returned by SP
    }
}
