using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class ClientViewModel
    {
        public int clientId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Client name must have between 2 and 100 letters")]
        [Display(Name = "Client name")]
        public string Name { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Client brand must have between 2 and 10 letters")]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [DisplayName("DR Accs")]
        public int AccNo { get; set; }

        public AddressViewModel Address { get; set; } = new(); // Embedded Address Object

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        public List<ClientContactIndexViewModel> ContactsList { get; set; } = new();
    }
}
