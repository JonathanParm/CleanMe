using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class ExportClientScheduleToExcelViewModel
    {
        [Display(Name = "Date Range Type")]
        public string DateRangeType { get; set; } = "Month"; // Default to "Month"

        [Display(Name = "Year")]
        public int? Year { get; set; }

        [Display(Name = "Month")]
        public int? Month { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public IEnumerable<SelectListItem>? YearList { get; set; }
        public IEnumerable<SelectListItem>? MonthList { get; set; }

        public int? ClientId { get; set; } 

        public IEnumerable<SelectListItem>? Clients { get; set; }
    }
}