using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMe.Application.ViewModels
{
    public class AreaViewModel
    {
        public int areaId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Area name must have between 2 and 50 letters")]
        [Display(Name = "Area name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Region")]
        public int regionId { get; set; }

        [Required]
        [Display(Name = "Report code")]
        public int ReportCode { get; set; }

        [Required]
        [Display(Name = "Sort order")]
        public int SequenceOrder { get; set; }

        [Required]
        [Display(Name = "Seq no")]
        public int SeqNo { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        public string? RegionName { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; } = new List<SelectListItem>();

        public List<AssetLocationIndexViewModel> AssetLocationsList { get; set; } = new();

    }
}
