using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Entities
{
    [Table("AmendmentTypes")]
    public class AmendmentType
    {
        [Key]
        [DisplayName("Asset type")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int amendmentTypeId { get; set; }

        [Required(ErrorMessage = "Must have an amendment type name")]
        [DisplayName("Type")]
        [Column(TypeName = "varchar")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Amendment type Name must have between 3 and 50 letters")]
        public string Name { get; set; }

        [DisplayName("Description")]
        [Column(TypeName = "varchar")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Amendment type Name must have between 3 and 1000 letters")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Sort order")]
        public int SortOrder { get; set; }

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
        [DisplayName("Added by")]
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
        public ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();
    }
}
