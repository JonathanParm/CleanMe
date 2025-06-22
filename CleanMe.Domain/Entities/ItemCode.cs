using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Entities
{
    [Table("ItemCodes")]
    public class ItemCode
    {
        [Key]
        [DisplayName("Item code")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int itemCodeId { get; set; }

        [Required(ErrorMessage = "Must have an item code")]
        [DisplayName("Code")]
        [Column(TypeName = "varchar")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Item code must have between 3 and 10 letters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Must have an item name")]
        [DisplayName("Name")]
        [Column(TypeName = "varchar")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Item code must have between 3 and 20 letters")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Item must have an item description")]
        [DisplayName("Item description")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "An item description must have between 3 and 100 letters")]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "Item must have a purchase description")]
        [DisplayName("Purchase description")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A purchases description must have between 3 and 100 letters")]
        public string PurchasesDescription { get; set; }

        [Required(ErrorMessage = "Item must have a sales description")]
        [DisplayName("Sales description")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A sales description must have between 3 and 100 letters")]
        public string SalesDescription { get; set; }

        [Required]
        [DisplayName("Purchases unit rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal PurchasesUnitRate { get; set; }

        [Required]
        [DisplayName("Sales unit rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal SalesUnitRate { get; set; }

        [DisplayName("Purchases XERO account")]
        public int? PurchasesXeroAccount { get; set; }

        [DisplayName("Sales XERO account")]
        public int? SalesXeroAccount { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [DisplayName("Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [DisplayName("Added at")]
        public DateTime AddedAt { get; set; }

        [Required]
        [DisplayName("Added by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string AddedById { get; set; }

        [Required]
        [DisplayName("Updated at")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [DisplayName("Updated by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string UpdatedById { get; set; }

        // Navigation property
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
        public ICollection<ItemCodeRate> ItemCodeRates { get; set; } = new List<ItemCodeRate>();
    }
}