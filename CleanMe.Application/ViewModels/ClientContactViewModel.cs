using CleanMe.Domain.Common;
using CleanMe.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Application.ViewModels
{
    public class ClientContactViewModel
    {
        public int clientContactId { get; set; }

        public int clientId { get; set; }

        [Display(Name = "First name")]
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Family name")]
        [Required]
        [StringLength(50, ErrorMessage = "Family/last name cannot exceed 50 characters.")]
        [MinLength(2, ErrorMessage = "Family/last name must be at least 2 characters.")]
        public string FamilyName { get; set; } = string.Empty;

        [Display(Name = "Mobile phone")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Mobile phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Mobile phone number must be at least 6 digits.")]
        public string? PhoneMobile { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [StringLength(256, ErrorMessage = "Email address cannot have more than 256 digits")]
        [MinLength(6, ErrorMessage = "An email address must be at least 6 digits.")]
        [Remote(action: "IsEmailAvailable", controller: "ClientContact", AdditionalFields = "clientContactId", ErrorMessage = "Email is already in use.")]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Job title")]
        [StringLength(20)]
        public string? JobTitle { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public string? ApplicationUserId { get; set; }
        public bool HasApplicationUser => !string.IsNullOrWhiteSpace(ApplicationUserId);
        
        [ScaffoldColumn(false)]
        public bool ShowPasswordSection { get; set; } = false;

        [DataType(DataType.Password)]
        public string? Password { get; set; } = string.Empty;

        public ChangePasswordViewModel ChangePassword { get; set; } = new();
 
        [Display(Name = "Client contact name")]
        public string? Fullname
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                    return FamilyName;

                return $"{FirstName} {FamilyName}";
            }
        }
 
        [Display(Name = "Contact detail")]
        public string? ContactDetail
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PhoneMobile))
                    return PhoneMobile;

                return Email;
            }
        }

        [Display(Name = "Client")]
        public string? ClientName { get; set; }
    }
}
