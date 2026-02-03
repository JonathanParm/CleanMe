using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Area")]
        public string AreaName { get; set; }

        [Display(Name = "MD reference")]
        public string MdReference { get; set; }

        [Display(Name = "Client name")]
        public string ClientName { get; set; }

        [Display(Name = "Client reference")]
        public string ClientReference { get; set; }

        [Display(Name = "Asset location")]
        public string AssetLocation { get; set; }

        [Display(Name = "Item code")]
        public string ItemCode { get; set; }
    }
}
