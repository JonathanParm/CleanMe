using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class ItemCodeRateIndexViewModel
    {
        [Display(Name = "ID")]
        public int itemCodeRateId { get; set; }

        [Display(Name = "Rate")]
        public string RateName { get; set; }

        [Display(Name = "Item code")]
        public string ItemCodeName { get; set; }

        [Display(Name = "Clean frequency")]
        public string CleanFreqName { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }

        [Display(Name = "Rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Rate { get; set; }

        [Display(Name = "Default")]
        public bool IsDefault { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
