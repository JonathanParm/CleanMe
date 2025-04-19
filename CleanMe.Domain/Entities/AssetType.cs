using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Domain.Entities
{
    [Table("AssetTypes")]
    public class AssetType
    {
        [Key]
        [DisplayName("Asset type")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int assetTypeId { get; set; }

        [Required(ErrorMessage = "Must have an asset type name")]
        [DisplayName("Type")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Asset Type Name must have between 3 and 50 letters")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        // Foreign key property linking to the parent
        //[ForeignKey("StockCode")]
        [DisplayName("Stock code")]
        public int StockCodeId { get; set; }
        // Navigation property representing the parent
        //[DisplayName("Stock code")]
        //public StockCode? StockCode { get; set; }

        //// Navigation property representing the collection of children
        //public ICollection<ServiceRate>? ServiceRates { get; set; }

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
    }
}