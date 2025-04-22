using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class StockCodeViewModel
    {
        public int stockCodeId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Code must have between 3 and 50 letters")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must have between 3 and 500 letters")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        public List<AssetTypeIndexViewModel> AssetTypesList { get; set; } = new();
    }
}
