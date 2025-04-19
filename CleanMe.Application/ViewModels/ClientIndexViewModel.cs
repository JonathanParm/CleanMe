using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class ClientIndexViewModel
    {
        [Display(Name = "ID")]
        public int clientId { get; set; }

        [Display(Name = "Client")]
        public string Name { get; set; }

        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [DisplayName("DR Accs")]
        public int AccNo { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
