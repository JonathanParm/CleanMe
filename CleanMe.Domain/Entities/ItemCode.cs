using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    [Table("ItemCodes")]
    public class ItemCode
    {
        [Key]
        [Display(Name = "Item code")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int itemCodeId { get; set; }

        [Required(ErrorMessage = "Must have an item code")]
        [Display(Name = "Code")]
        [Column(TypeName = "varchar")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Item code must have between 3 and 10 letters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Must have an item name")]
        [Display(Name = "Name")]
        [Column(TypeName = "varchar")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Item code must have between 3 and 20 letters")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Item must have an item description")]
        [Display(Name = "Item description")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "An item description must have between 3 and 100 letters")]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "Item must have a purchase description")]
        [Display(Name = "Purchase description")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A purchases description must have between 3 and 100 letters")]
        public string PurchasesDescription { get; set; }

        [Required(ErrorMessage = "Item must have a sales description")]
        [Display(Name = "Sales description")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A sales description must have between 3 and 100 letters")]
        public string SalesDescription { get; set; }

        [Required]
        [Display(Name = "Purchases unit rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal PurchasesUnitRate { get; set; }

        [Required]
        [Display(Name = "Sales unit rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal SalesUnitRate { get; set; }

        [Display(Name = "Purchases XERO account")]
        public int? PurchasesXeroAccount { get; set; }

        [Display(Name = "Sales XERO account")]
        public int? SalesXeroAccount { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Display(Name = "Added at")]
        public DateTime AddedAt { get; set; }

        [Required]
        [Display(Name = "Added by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string AddedById { get; set; }

        [Required]
        [Display(Name = "Updated at")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [Display(Name = "Updated by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string UpdatedById { get; set; }

        // Navigation property
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
        public ICollection<ItemCodeRate> ItemCodeRates { get; set; } = new List<ItemCodeRate>();
    }
}