using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class ClientViewModel
    {
        public int clientId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Client name must have between 2 and 100 letters")]
        [Display(Name = "Client")]
        public string ClientName { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Client brand must have between 2 and 10 letters")]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Display(Name = "DR Accs")]
        public int AccNo { get; set; }

        [StringLength(50, ErrorMessage = "Client reference cannot have more than 50 letters")]
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        public AddressViewModel Address { get; set; } = new(); // Embedded Address Object

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<ClientContactIndexViewModel> ContactsList { get; set; } = new();
    }
}
