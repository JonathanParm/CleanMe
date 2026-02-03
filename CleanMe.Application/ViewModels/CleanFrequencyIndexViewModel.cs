using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class CleanFrequencyIndexViewModel
    {
        [Display(Name = "ID")]
        public int cleanFrequencyId { get; set; }

        [Display(Name = "Clean frequency name")]
        public string CleanFrequencyName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
