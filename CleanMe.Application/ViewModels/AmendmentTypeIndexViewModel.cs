using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class AmendmentTypeIndexViewModel
    {
        [Display(Name = "ID")]
        public int amendmentTypeId { get; set; }

        [Display(Name = "Amendment type")]
        public string AmendmentTypeName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
