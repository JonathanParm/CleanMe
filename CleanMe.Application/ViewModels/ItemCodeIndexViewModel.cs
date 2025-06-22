using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class ItemCodeIndexViewModel
    {
        [Display(Name = "ID")]
        public int itemCodeId { get; set; }

        [Display(Name = "Item code")]
        public string Code { get; set; }

        [Display(Name = "Item name")]
        public string ItemName { get; set; }

        [DisplayName("Item description")]
        public string ItemDescription { get; set; }

        [DisplayName("Purchases unit rate")]
        public decimal PurchasesUnitRate { get; set; }

        [DisplayName("Purchases XERO account")]
        public int? PurchasesXeroAccount { get; set; }

        [DisplayName("Sales unit rate")]
        public decimal SalesUnitRate { get; set; }

        [DisplayName("Sales XERO account")]
        public int? SalesXeroAccount { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
