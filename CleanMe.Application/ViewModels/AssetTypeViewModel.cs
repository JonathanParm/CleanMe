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
    public class AssetTypeViewModel
    {
        public int assetTypeId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Asset type name must have between 2 and 50 letters")]
        [Display(Name = "Asset type name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;
    }
}
