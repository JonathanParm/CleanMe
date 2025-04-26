using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class AmendmentIndexViewModel
    {
        [Display(Name = "ID")]
        public int amendmentId { get; set; }

        [Display(Name = "Source name")]
        public string SourceName { get; set; }

        [Display(Name = "Client name")]
        public string ClientName { get; set; }

        [Display(Name = "Area name")]
        public string AreaName { get; set; }

        [Display(Name = "Asset location")]
        public string LocationName { get; set; }

        [Display(Name = "MD reference")]
        public string MdReference { get; set; }

        [Display(Name = "Client reference")]
        public string ClientReference { get; set; }

        [Display(Name = "Amendment summary")]
        public string AmendmentSummary { get; set; }
    }
}
