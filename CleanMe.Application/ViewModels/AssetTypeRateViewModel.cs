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
    public class AssetTypeRateViewModel
    {
        public int assetTypeRateId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Asset type rate name must have between 2 and 50 letters")]
        [Display(Name = "Rate name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Asset type rate description must have between 2 and 100 letters")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public int? assetTypeId { get; set; }

        [Required]
        [Display(Name = "Clean Frequency")]
        public int? cleanFrequencyId { get; set; }

        [Required]
        [Display(Name = "Client")]
        public int? clientId { get; set; }

        [Required]
        [DisplayName("Rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Rate { get; set; }

        [Required]
        [DisplayName("Default rate")]
        public bool IsDefault { get; set; } = false;

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Asset Type")]
        public string? AssetTypeName { get; set; }
        public IEnumerable<SelectListItem> AssetTypes { get; set; } = new List<SelectListItem>();


        [Display(Name = "Clean Frequency")]
        public string? CleanFreqName { get; set; }
        public IEnumerable<SelectListItem> CleanFrequencies { get; set; } = new List<SelectListItem>();

        [Display(Name = "Client")]
        public string? ClientName { get; set; }
        public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();

    }
}
