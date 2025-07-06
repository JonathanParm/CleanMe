using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Application.ViewModels
{
    public class ItemCodeRateViewModel
    {
        public int itemCodeRateId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Item code rate name must have between 2 and 50 letters")]
        [Display(Name = "Rate name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Item code rate description must have between 2 and 100 letters")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Item code")]
        public int? itemCodeId { get; set; }

        [Required]
        [Display(Name = "Clean Frequency")]
        public int? cleanFrequencyId { get; set; }

        [Required]
        [Display(Name = "Client")]
        public int? clientId { get; set; }

        [Required]
        [Display(Name = "Rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Rate { get; set; }

        [Required]
        [Display(Name = "Default rate")]
        public bool IsDefault { get; set; } = false;

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Item code")]
        public string? ItemCodeName { get; set; }
        public IEnumerable<SelectListItem> ItemCodes { get; set; } = new List<SelectListItem>();


        [Display(Name = "Clean Frequency")]
        public string? CleanFreqName { get; set; }
        public IEnumerable<SelectListItem> CleanFrequencies { get; set; } = new List<SelectListItem>();

        [Display(Name = "Client")]
        public string? ClientName { get; set; }
        public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();

    }
}
