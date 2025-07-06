using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Application.ViewModels
{
    public class ItemCodeViewModel
    {
        public int itemCodeId { get; set; }

        [Required(ErrorMessage = "Must have an item code")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Must have an item name")]
        [Display(Name = "Name")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Item must have an item description")]
        [Display(Name = "Item description")]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "Item must have a purchase description")]
        [Display(Name = "Purchase description")]
        public string PurchasesDescription { get; set; }

        [Required(ErrorMessage = "Item must have a sales description")]
        [Display(Name = "Sales description")]
        public string SalesDescription { get; set; }

        [Required]
        [Display(Name = "Purchases unit rate")]
        public decimal PurchasesUnitRate { get; set; }

        [Required]
        [Display(Name = "Sales unit rate")]
        public decimal SalesUnitRate { get; set; }

        [Display(Name = "Purchases XERO account")]
        public int? PurchasesXeroAccount { get; set; }

        [Display(Name = "Sales XERO account")]
        public int? SalesXeroAccount { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        //public List<ItemCodeRateIndexViewModel> ItemCodeRatesList { get; set; } = new();
    }
}
