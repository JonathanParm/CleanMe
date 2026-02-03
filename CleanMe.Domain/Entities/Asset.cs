using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    [Table("Assets")]
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int assetId { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "MD reference must have between 2 and 50 letters")]
        [Display(Name = "MD reference")]
        public string MdReference { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Asset name must have between 2 and 50 letters")]
        [Display(Name = "Asset name")]
        public string AssetName { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Client reference must have between 2 and 50 letters")]
        [Display(Name = "Client Reference")]
        public string? ClientReference { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Asset position must have between 2 and 50 letters")]
        [Display(Name = "Asset position")]
        public string? Position { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Description of how to find asset must have between 2 and 200 letters")]
        [Display(Name = "Access to asset")]
        public string? Access { get; set; }

        [ForeignKey("Client")]
        [Display(Name = "Client")]
        public int clientId { get; set; }

        // Navigation property
        [Display(Name = "Client")]
        [ForeignKey(nameof(clientId))]
        public Client? Client { get; set; }

        [ForeignKey("AssetLocation")]
        [Display(Name = "Asset Location")]
        public int assetLocationId { get; set; }

        // Navigation property
        [Display(Name = "Asset Location")]
        [ForeignKey(nameof(assetLocationId))]
        public AssetLocation? AssetLocation { get; set; }

        [ForeignKey("ItemCode")]
        [Display(Name = "Item code")]
        public int itemCodeId { get; set; }

        // Navigation property
        [Display(Name = "Item code")]
        [ForeignKey(nameof(itemCodeId))]
        public ItemCode? ItemCode { get; set; }

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
        public ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();
    }
}
