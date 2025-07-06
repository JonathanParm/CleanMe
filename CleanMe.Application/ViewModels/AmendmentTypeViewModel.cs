using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class AmendmentTypeViewModel
    {
        public int amendmentTypeId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Code must have between 3 and 50 letters")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Description must have between 3 and 1000 letters")]
        public string Description { get; set; }

        [Display(Name = "Sort order")]
        public int SortOrder { get; set; }

        [Display(Name = "Has staff")]
        public bool HasStaffId { get; set; } = false;

        [Display(Name = "Has client")]
        public bool HasClientId { get; set; } = false;

        [Display(Name = "Has region")]
        public bool HasRegionId { get; set; } = false;

        [Display(Name = "Has area")]
        public bool HasAreaId { get; set; } = false;

        [Display(Name = "Has asset location")]
        public bool HasAssetLocationId { get; set; } = false;

        [Display(Name = "Has item type")]
        public bool HasItemCodeId { get; set; } = false;

        [Display(Name = "Has asset")]
        public bool HasAssetId { get; set; } = false;

        [Display(Name = "Has clean frequency")]
        public bool HasCleanFrequencyId { get; set; } = false;

        [Display(Name = "Has rate")]
        public bool HasRate { get; set; } = false;

        [Display(Name = "Has access")]
        public bool HasAccess { get; set; } = false;

        [Display(Name = "Has 'Is accessible'")]
        public bool HasIsAccessable { get; set; } = false;

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<AmendmentIndexViewModel> AmendmentsList { get; set; } = new();
    }
}
