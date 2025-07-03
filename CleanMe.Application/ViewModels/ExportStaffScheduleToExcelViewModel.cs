using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class ExportStaffScheduleToExcelViewModel
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

        public int? StaffId { get; set; }
        public int? ClientId { get; set; } 
        public int? RegionId { get; set; }
        public int? AreaId { get; set; } 
        public int? AssetLocationId { get; set; }
        public int? AssetId { get; set; }

        public IEnumerable<SelectListItem>? Cleaners { get; set; }
        public IEnumerable<SelectListItem>? Clients { get; set; }
        public IEnumerable<SelectListItem>? Regions { get; set; }
        public IEnumerable<SelectListItem>? Areas { get; set; }
        public IEnumerable<SelectListItem>? AssetLocations { get; set; }
        public IEnumerable<SelectListItem>? Assets { get; set; }
    }
}