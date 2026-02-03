using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    [Table("Areas")]
    public class Area
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int areaId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Area name must have between 2 and 100 letters")]
        [Display(Name = "Area name")]
        public string AreaName { get; set; }

        [ForeignKey("Region")]
        [Display(Name = "Region")]
        public int regionId { get; set; }

        // Navigation property
        [Display(Name = "Region")]
        [ForeignKey(nameof(regionId))]
        public virtual Region? Region { get; set; }

        [Required]
        [Display(Name = "Report code")]
        public int ReportCode { get; set; }

        [Required]
        [Display(Name = "Sort order")]
        public int SortOrder { get; set; }

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

        // Navigation property
        public ICollection<AssetLocation> AssetLocations { get; set; } = new List<AssetLocation>();

        // Navigation property
        public ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();
    }
}

