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
    public class CleanFrequencyViewModel
    {
        public int cleanFrequencyId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Clean frequency name must have between 2 and 50 letters")]
        [Display(Name = "Clean frequency name")]
        public string Name { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Clean frequency name must have between 2 and 500 letters")]
        [Display(Name = "Clean frequency description")]
        public string Description { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Code must have 2 letters")]
        [Display(Name = "Report code")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Sort order")]
        public int SortOrder { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;
    }
}
