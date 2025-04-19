namespace CleanMe.Application.ViewModels
{
    public class ClientContactIndexViewModel
    {
        public int clientContactId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string PhoneMobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}