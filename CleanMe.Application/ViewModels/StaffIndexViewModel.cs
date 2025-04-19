namespace CleanMe.Application.ViewModels
{
    public class StaffIndexViewModel
    {
        public int staffId { get; set; }
        public string StaffNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string WorkRole { get; set; } = string.Empty;
        public string ContactDetail { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}