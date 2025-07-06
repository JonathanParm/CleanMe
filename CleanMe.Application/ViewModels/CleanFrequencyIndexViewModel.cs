using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class CleanFrequencyIndexViewModel
    {
        [Display(Name = "ID")]
        public int cleanFrequencyId { get; set; }

        [Display(Name = "Clean frequency name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Sort order")]
        public int SequenceOrder { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
