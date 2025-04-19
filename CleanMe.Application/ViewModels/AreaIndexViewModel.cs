using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class AreaIndexViewModel
    {
        [Display(Name = "ID")]
        public int areaId { get; set; }

        [Display(Name = "Region")]
        public string RegionName { get; set; }

        [Display(Name = "Area name")]
        public string Name { get; set; }

        [Display(Name = "Report code")]
        public int ReportCode { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
