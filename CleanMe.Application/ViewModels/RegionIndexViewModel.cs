using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class RegionIndexViewModel
    {
        [Display(Name = "ID")]
        public int regionId { get; set; }

        [Display(Name = "Region name")]
        public string Name { get; set; }

        [Display(Name = "Report code")]
        public string ReportCode { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
