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

        [Display(Name = "Item description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Purchases unit rate")]
        public decimal PurchasesUnitRate { get; set; }

        [Display(Name = "Purchases XERO account")]
        public int? PurchasesXeroAccount { get; set; }

        [Display(Name = "Sales unit rate")]
        public decimal SalesUnitRate { get; set; }

        [Display(Name = "Sales XERO account")]
        public int? SalesXeroAccount { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
