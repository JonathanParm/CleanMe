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
    [Table("Amendments")]
    public class Amendment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int amendmentId { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100, ErrorMessage = "Source name of amendment cannot exceed 100 characters")]
        [MinLength(2, ErrorMessage = "Source name of amendment must be at least 2 characters.")]
        [Display(Name = "Name of source")]
        public string? AmendmentSourceName { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("Client")]
        [DisplayName("Client")]
        public int? clientId { get; set; }
        // Navigation property representing the parent
        [DisplayName("Client")]
        public Client? Client { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("Area")]
        [DisplayName("Area")]
        public int? areaId { get; set; }
        // Navigation property representing the parent
        [DisplayName("Area")]
        public Area? Area { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("AssetLocation")]
        [DisplayName("Asset location")]
        public int? assetLocationId { get; set; }
        // Navigation property representing the parent
        [DisplayName("Asset location")]
        public AssetLocation? AssetLocation { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("Asset")]
        [DisplayName("Asset")]
        public int? assetId { get; set; }
        // Navigation property representing the parent
        [DisplayName("Asset")]
        public Asset? Asset { get; set; }

        // Optional Foreign key property linking to the parent
        [ForeignKey("Staff")]
        [Display(Name = "Staff Number")]
        public int? staffId { get; set; }
        // Navigation property representing the parent
        [DisplayName("Staff")]
        public Staff? Staff { get; set; }

        // Foreign key property linking to the parent
        [ForeignKey("Frequency")]
        [DisplayName("Clean frequency")]
        public int? cleanFrequencyId { get; set; }
        // Navigation property representing the parent
        [DisplayName("Clean frequency")]
        public CleanFrequency? CleanFrequency { get; set; }

        [DisplayName("Rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Rate { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Description of how to find asset must have between 2 and 200 letters")]
        [Display(Name = "Access to asset")]
        public string? Access { get; set; }

        [Display(Name = "Is Accessable")]
        public bool IsAccessable { get; set; } = true;

        [DisplayName("Start on")]
        public DateOnly? StartOn { get; set; }

        [DisplayName("Finish on")]
        public DateOnly? FinishOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(2000, MinimumLength = 2, ErrorMessage = "Comment about amendment to schedule must have between 2 and 2000 letters")]
        [Display(Name = "Comment about amendment")]
        public string? Comment { get; set; }

        [DisplayName("Invoiced")]
        public DateOnly? Invoiced { get; set; }

        [Required]
        [DisplayName("Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [DisplayName("Added at")]
        public DateTime AddedAt { get; set; }

        [Required]
        [DisplayName("Created by")]
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