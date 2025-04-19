using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class AssetTypeIndexViewModel
    {
        [Display(Name = "ID")]
        public int assetTypeId { get; set; }

        [Display(Name = "Asset type name")]
        public string Name { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
