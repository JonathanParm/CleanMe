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
        public string Name { get; set; }

        [ForeignKey("Region")]
        [DisplayName("Region")]
        public int regionId { get; set; }

        // Navigation property
        [DisplayName("Region")]
        [ForeignKey(nameof(regionId))]
        public virtual Region? Region { get; set; }

        [Required]
        [Display(Name = "Report code")]
        public int ReportCode { get; set; }

        [Required]
        [Display(Name = "Sort order")]
        public int SequenceOrder { get; set; }

        // temp for initial data load. previous AreaId.
        [DisplayName("Seq no")]
        public int SeqNo { get; set; }

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

        // Navigation property
        public ICollection<AssetLocation> AssetLocations { get; set; } = new List<AssetLocation>();

    }
}

