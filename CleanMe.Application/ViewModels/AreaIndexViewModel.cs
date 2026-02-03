using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class AreaIndexViewModel
    {
        [Display(Name = "ID")]
        public int areaId { get; set; }

        [Display(Name = "Region")]
        public string RegionName { get; set; }

        [Display(Name = "Area")]
        public string AreaName { get; set; }

        [Display(Name = "Report code")]
        public int ReportCode { get; set; }

        [Display(Name = "Cleaner")]
        public string CleanerName { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
