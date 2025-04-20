using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanMe.Domain.Common;

namespace CleanMe.Domain.Entities
{
    [Table("AssetLocations")]
    public class AssetLocation
    {
        [Key]
        [DisplayName("Asset location")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int assetLocationId { get; set; }

        [Required(ErrorMessage = "Must have an asset location description")]
        [DisplayName("Location")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Asset location description must have between 3 and 200 letters")]
        public string Description { get; set; }

        public Address Address { get; set; } = new Address(); // Embedded Address Object

        [Required]
        [Display(Name = "Sort order")]
        public int SequenceOrder { get; set; }

        [DisplayName("Seq no")]
        public int SeqNo { get; set; }

        [DisplayName("Report code")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Report code must have between 2 and 20 letters")]
        public string ReportCode { get; set; }

        [DisplayName("ACC no")]
        public int AccNo { get; set; }

        [ForeignKey("Area")]
        [DisplayName("Area")]
        public int areaId { get; set; }

        // Navigation property
        [DisplayName("Area")]
        [ForeignKey(nameof(areaId))]
        public virtual Area? Area { get; set; }

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

        //Navigation property
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}