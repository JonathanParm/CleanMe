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
    [Table("CleanFrequencies")]
    public class CleanFrequency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int cleanFrequencyId { get; set; }

        [Required(ErrorMessage = "Must have frequency")]
        [DisplayName("Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Frequency must have between 3 and 50 letters")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Description must have between 2 and 50 letters")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Code")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(2, ErrorMessage = "Code must have between 1 and 2 letters")]
        public string Code { get; set; }

        [Required]
        [DisplayName("Sort order")]
        public int SequenceOrder { get; set; }

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
        public ICollection<AssetTypeRate> AssetTypeRates { get; set; } = new List<AssetTypeRate>();

        //// Navigation property
        //public ICollection<ScheduleAmendment> ScheduleAmendments { get; set; } = new List<ScheduleAmendment>();
    }
}
