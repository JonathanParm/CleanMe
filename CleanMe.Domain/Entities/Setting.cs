using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    [Table("Settings")]
    public class Setting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "ID")]
        public int settingId { get; set; }

        [Required(ErrorMessage = "Must have Export Excel folder path")]
        [Display(Name = "Export to Excel folder")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Export to Excel folder path must have between 3 and 100 letters")]
        public string ExcelExportPath { get; set; }

        [Required]
        [Display(Name = "Updated at")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [Display(Name = "Updated by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string UpdatedById { get; set; }
    }
}
