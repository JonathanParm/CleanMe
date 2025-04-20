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
    public class AssetViewModel
    {
        public int assetId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Area name must have between 2 and 50 letters")]
        [Display(Name = "Area name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Client")]
        public int clientId { get; set; }

        [Display(Name = "Client")]
        public string? ClientName { get; set; }

        [Required]
        [Display(Name = "Asset Location")]
        public int assetLocationId { get; set; }

        [Display(Name = "Asset Location")]
        public string? AssetLocationName { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public int assetTypeId { get; set; }

        [Display(Name = "Asset Type")]
        public string? AssetTypeName { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "MD reference must have between 2 and 50 letters")]
        [Display(Name = "MD reference")]
        public string MdReference { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Client reference must have between 2 and 50 letters")]
        [Display(Name = "Client Reference")]
        public string? ClientReference { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Asset position must have between 2 and 50 letters")]
        [Display(Name = "Asset position")]
        public string? Position { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Description of how to find asset must have between 2 and 200 letters")]
        [Display(Name = "Access to asset")]
        public string? Access { get; set; }

        [Display(Name = "Inaccessable")]
        public bool Inaccessable { get; set; } = false;

        public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AssetLocations { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AssetTypes { get; set; } = new List<SelectListItem>();
    }
}
