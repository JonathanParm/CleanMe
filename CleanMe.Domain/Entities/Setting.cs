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
    [Table("Settings")]
    public class Setting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "ID")]
        public int settingId { get; set; }

        [Required(ErrorMessage = "Must have Export Excel folder path")]
        [DisplayName("Export to Excel folder")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Export to Excel folder path must have between 3 and 100 letters")]
        public string ExcelExportPath { get; set; }

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
