using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CleanMe.Domain.Common;

namespace CleanMe.Domain.Entities
{
    [Table("AssetLocations")]
    public class AssetLocation
    {
        [Key]
        [Display(Name = "Asset location")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int assetLocationId { get; set; }

        [Required(ErrorMessage = "Must have an asset location description")]
        [Display(Name = "Location")]
        [Column(TypeName = "varchar")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Asset location description must have between 3 and 200 letters")]
        public string Description { get; set; }

        public Address Address { get; set; } = new Address(); // Embedded Address Object

        [Required]
        [Display(Name = "Sort order")]
        public int SortOrder { get; set; }

        [Display(Name = "Report code")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Report code must have between 2 and 20 letters")]
        public string ReportCode { get; set; }

        [Display(Name = "ACC no")]
        public int AccNo { get; set; }

        [ForeignKey("Area")]
        [Display(Name = "Area")]
        public int areaId { get; set; }

        // Navigation property
        [Display(Name = "Area")]
        [ForeignKey(nameof(areaId))]
        public virtual Area? Area { get; set; }

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

        //Navigation property
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();

        // Navigation property
        public ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();
    }
}