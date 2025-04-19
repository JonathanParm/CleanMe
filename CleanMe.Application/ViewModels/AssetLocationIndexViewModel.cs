using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class AssetLocationIndexViewModel
    {
        [Display(Name = "Asset location")]
        public int assetLocationId { get; set; }

        [Display(Name = "Area")]
        public string AreaName { get; set; }

        [Display(Name = "Area description")]
        public string Description { get; set; }

        [Display(Name = "Report code")]
        public string ReportCode { get; set; }

        [Display(Name = "Town, suburb")]
        public string TownSuburb { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
