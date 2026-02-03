using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Domain.Entities
{
    [Table("AmendmentTypes")]
    public class AmendmentType
    {
        [Key]
        [Display(Name = "Asset type")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int amendmentTypeId { get; set; }

        [Required(ErrorMessage = "Must have an amendment type name")]
        [Display(Name = "Amendment type")]
        [Column(TypeName = "varchar")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Amendment type Name must have between 3 and 50 letters")]
        public string AmendmentTypeName { get; set; }

        [Display(Name = "Description")]
        [Column(TypeName = "varchar")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Amendment type Name must have between 3 and 1000 letters")]
        public string Description { get; set; }

        [Display(Name = "Client")]
        public bool HasClientId { get; set; } = false;

        [Display(Name = "Area")]
        public bool HasAreaId { get; set; } = false;

        [Display(Name = "Asset location")]
        public bool HasAssetLocationId { get; set; } = false;

        [Display(Name = "Item type")]
        public bool HasItemCodeId { get; set; } = false;

        [Display(Name = "Asset")]
        public bool HasAssetId { get; set; } = false;

        [Display(Name = "Clean frequency")]
        public bool HasCleanFrequencyId { get; set; } = false;

        [Display(Name = "Staff")]
        public bool HasStaffId { get; set; } = false;

        [Display(Name = "Rate")]
        public bool HasRate { get; set; } = false;

        [Display(Name = "Access")]
        public bool HasAccess { get; set; } = false;

        [Display(Name = "Is accessible")]
        public bool HasIsAccessable { get; set; } = false;

        [Display(Name = "Is default")]
        public bool HasIsDefault { get; set; } = false;

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
