using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class AmendmentIndexViewModel
    {
        [Display(Name = "ID")]
        public int amendmentId { get; set; }

        [Display(Name = "Source")]
        public string SourceName { get; set; }

        [Display(Name = "Amendment type")]
        public string AmendmentTypeName { get; set; }

        [Display(Name = "Client")]
        public string ClientName { get; set; }

        [Display(Name = "Area")]
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
