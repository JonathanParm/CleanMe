using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    [Table("CleanFrequencies")]
    public class CleanFrequency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int cleanFrequencyId { get; set; }

        [Required(ErrorMessage = "Must have frequency")]
        [Display(Name = "Clean frequency")]
        [Column(TypeName = "varchar")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Frequency must have between 3 and 50 letters")]
        public string CleanFrequencyName { get; set; }

        [Required]
        [Display(Name = "Description")]
        [Column(TypeName = "varchar")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Description must have between 2 and 50 letters")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Code")]
        [Column(TypeName = "varchar")]
        [StringLength(2, ErrorMessage = "Code must have between 1 and 2 letters")]
        public string Code { get; set; }

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
        public ICollection<ItemCodeRate> AssetTypeRates { get; set; } = new List<ItemCodeRate>();

        // Navigation property
        public ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();
    }
}
