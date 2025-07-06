using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class RegionViewModel
    {
        public int regionId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Region name must have between 2 and 50 letters")]
        [Display(Name = "Region name")]
        public string Name { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Code must have 2 letters")]
        [Display(Name = "Report code")]
        public string ReportCode { get; set; }

        [Required]
        [Display(Name = "Sort order")]
        public int SortOrder { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<AreaIndexViewModel> AreasList { get; set; } = new();
    }
}
