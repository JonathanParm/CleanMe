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
    public class AssetTypeViewModel
    {
        public int assetTypeId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Asset type name must have between 2 and 50 letters")]
        [Display(Name = "Asset type name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Stock code")]
        public int stockCodeId { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        public string? StockCodeName { get; set; }

        public IEnumerable<SelectListItem> StockCodes { get; set; } = new List<SelectListItem>();

    }
}
