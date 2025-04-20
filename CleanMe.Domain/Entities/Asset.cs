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
    [Table("Assets")]
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int assetId { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "MD reference must have between 2 and 50 letters")]
        [Display(Name = "MD reference")]
        public string MdReference { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Asset name must have between 2 and 50 letters")]
        [Display(Name = "Asset name")]
        public string Name { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Client reference must have between 2 and 50 letters")]
        [Display(Name = "Client Reference")]
        public string? ClientReference { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Asset position must have between 2 and 50 letters")]
        [Display(Name = "Asset position")]
        public string? Position { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Description of how to find asset must have between 2 and 200 letters")]
        [Display(Name = "Access to asset")]
        public string? Access { get; set; }

        [Display(Name = "Inaccessable")]
        public bool Inaccessable { get; set; } = false;

        [ForeignKey("Client")]
        [DisplayName("Client")]
        public int clientId { get; set; }

        // Navigation property
        [DisplayName("Client")]
        [ForeignKey(nameof(clientId))]
        public virtual Client? Client { get; set; }

        [ForeignKey("AssetLocation")]
        [DisplayName("Asset Location")]
        public int assetLocationId { get; set; }

        // Navigation property
        [DisplayName("Asset Location")]
        [ForeignKey(nameof(assetLocationId))]
        public virtual AssetLocation? AssetLocation { get; set; }

        [ForeignKey("AssetType")]
        [DisplayName("Asset Type")]
        public int assetTypeId { get; set; }

        // Navigation property
        [DisplayName("Asset Type")]
        [ForeignKey(nameof(assetTypeId))]
        public virtual AssetType? AssetType { get; set; }

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
