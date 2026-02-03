using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class RegionIndexViewModel
    {
        [Display(Name = "ID")]
        public int regionId { get; set; }

        [Display(Name = "Region name")]
        public string RegionName { get; set; }

        [Display(Name = "Report code")]
        public string ReportCode { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
