using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Application.ViewModels
{
    public class ItemCodeViewModel
    {
        public int itemCodeId { get; set; }

        [Required(ErrorMessage = "Must have an item code")]
        [DisplayName("Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Must have an item name")]
        [DisplayName("Name")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Item must have an item description")]
        [DisplayName("Item description")]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "Item must have a purchase description")]
        [DisplayName("Purchase description")]
        public string PurchasesDescription { get; set; }

        [Required(ErrorMessage = "Item must have a sales description")]
        [DisplayName("Sales description")]
        public string SalesDescription { get; set; }

        [Required]
        [DisplayName("Purchases unit rate")]
        public decimal PurchasesUnitRate { get; set; }

        [Required]
        [DisplayName("Sales unit rate")]
        public decimal SalesUnitRate { get; set; }

        [DisplayName("Purchases XERO account")]
        public int? PurchasesXeroAccount { get; set; }

        [DisplayName("Sales XERO account")]
        public int? SalesXeroAccount { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        //public List<ItemCodeRateIndexViewModel> ItemCodeRatesList { get; set; } = new();
    }
}
