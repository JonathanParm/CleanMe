using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class AssetIndexViewModel
    {
        [Display(Name = "ID")]
        public int assetId { get; set; }

        [Display(Name = "Asset")]
        public string AssetName { get; set; }

        [Display(Name = "Region")]
        public string RegionName { get; set; }

        [Display(Name = "MD Reference")]
        public string MdReference { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Client Reference")]
        public string ClientReference { get; set; }

        [Display(Name = "Asset Location")]
        public string AssetLocation { get; set; }

        [Display(Name = "Asset Type")]
        public string AssetType { get; set; }
    }
}
