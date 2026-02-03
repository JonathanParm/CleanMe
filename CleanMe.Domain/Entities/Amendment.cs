using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    [Table("Amendments")]
    public class Amendment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int amendmentId { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(100, ErrorMessage = "Source name of amendment cannot exceed 100 characters")]
        [MinLength(2, ErrorMessage = "Source name of amendment must be at least 2 characters.")]
        [Display(Name = "Name of source")]
        public string? AmendmentSourceName { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("AmendmentType")]
        [Display(Name = "Amendment type")]
        public int? amendmentTypeId { get; set; }
        // Navigation property representing the parent
        [Display(Name = "AmendmentType")]
        public AmendmentType? AmendmentType { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("Client")]
        [Display(Name = "Client")]
        public int? clientId { get; set; }
        // Navigation property representing the parent
        [Display(Name = "Client")]
        public Client? Client { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("Area")]
        [Display(Name = "Area")]
        public int? areaId { get; set; }
        // Navigation property representing the parent
        [Display(Name = "Area")]
        public Area? Area { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("AssetLocation")]
        [Display(Name = "Asset location")]
        public int? assetLocationId { get; set; }
        // Navigation property representing the parent
        [Display(Name = "Asset location")]
        public AssetLocation? AssetLocation { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("ItemCode")]
        [Display(Name = "Item code")]
        public int? itemCodeId { get; set; }
        // Navigation property representing the parent
        [Display(Name = "Item code")]
        public ItemCode? ItemCode { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("Asset")]
        [Display(Name = "Asset")]
        public int? assetId { get; set; }
        // Navigation property representing the parent
        [Display(Name = "Asset")]
        public Asset? Asset { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("Staff")]
        [Display(Name = "Staff Number")]
        public int? staffId { get; set; }
        // Navigation property representing the parent
        [Display(Name = "Staff")]
        public Staff? Staff { get; set; }

        // Foreign key property linking to the parent
        [ForeignKey("Frequency")]
        [Display(Name = "Clean frequency")]
        public int? cleanFrequencyId { get; set; }
        // Navigation property representing the parent
        [Display(Name = "Clean frequency")]
        public CleanFrequency? CleanFrequency { get; set; }

        [Display(Name = "Rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Rate { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Description of how to find asset must have between 2 and 200 letters")]
        [Display(Name = "Access to asset")]
        public string? Access { get; set; }

        [Display(Name = "Is Accessable")]
        public bool IsAccessable { get; set; } = true;

        [Display(Name = "Is default")]
        public bool IsDefault { get; set; } = false;

        [Display(Name = "Start on")]
        public DateTime? StartOn { get; set; }

        [Display(Name = "Finish on")]
        public DateTime? FinishOn { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(2000, MinimumLength = 2, ErrorMessage = "Comment about amendment to schedule must have between 2 and 2000 letters")]
        [Display(Name = "Comment about amendment")]
        public string? Comment { get; set; }

        [Display(Name = "Invoiced")]
        public DateTime? InvoicedOn { get; set; }

        [Required]
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Display(Name = "Added at")]
        public DateTime AddedAt { get; set; }

        [Required]
        [Display(Name = "Created by")]
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
    }
}