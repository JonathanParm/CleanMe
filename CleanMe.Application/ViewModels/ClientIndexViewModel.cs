using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class ClientIndexViewModel
    {
        [Display(Name = "ID")]
        public int clientId { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }

        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Display(Name = "DR Accs")]
        public int AccNo { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
