using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class ClientViewModel
    {
        [Display(Name = "ID")]
        public int clientId { get; set; }

        [Display(Name = "Client")]
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Client name must have between 2 and 100 letters")]
        public string ClientName { get; set; }

        [Display(Name = "Brand")]
        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Client brand must have between 2 and 10 letters")]
        public string Brand { get; set; }

        [Display(Name = "DR Accs")]

        public int AccNo { get; set; }

        [Display(Name = "Reference")]
        [StringLength(50, ErrorMessage = "Client reference cannot have more than 50 letters")]
        public string Reference { get; set; }

        public AddressViewModel Address { get; set; } = new(); // Embedded Address Object

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<ClientContactIndexViewModel> ContactsList { get; set; } = new();
    }
}
