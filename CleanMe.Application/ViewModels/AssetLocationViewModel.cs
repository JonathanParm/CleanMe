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
    public class AssetLocationViewModel
    {
        public int assetLocationId { get; set; }

        [Required(ErrorMessage = "Must have an asset location description")]
        [Display(Name = "Location")]
        [Column(TypeName = "varchar")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Asset location description must have between 3 and 200 letters")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Area")]
        public int areaId { get; set; }

        public AddressViewModel Address { get; set; } = new(); // Embedded Address Object

        [Required]
        [Display(Name = "Sort order")]
        public int SortOrder { get; set; }

        [Required]
        [Display(Name = "Report code")]
        [Column(TypeName = "varchar")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Asset location report code must have between 2 and 20 letters")]
        public string ReportCode { get; set; }

        [Display(Name = "ACC no")]
        public int AccNo { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public string? AreaName { get; set; }
    }
}
