using CleanMe.Domain.Common;
using CleanMe.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Application.ViewModels
{
    public class CompanyInfoViewModel
    {
        public int companyInfoId { get; set; }

        [Required(ErrorMessage = "Must have company name")]
        [Display(Name = "Name")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Company name must have between 3 and 100 letters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Must have phone number")]
        [Display(Name = "Phone")]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Phone number must have between 7 and 20 letters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Must have email address")]
        [Display(Name = "Email")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Email address must have between 3 and 100 letters")]
        public string Email { get; set; }

        public AddressViewModel Address { get; set; } = new(); // Embedded Address Object
    }
}
