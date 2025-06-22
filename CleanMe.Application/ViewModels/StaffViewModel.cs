using CleanMe.Domain.Common;
using CleanMe.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Application.ViewModels
{
    public class StaffViewModel
    {
        public int StaffId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Staff number cannot be zero.")]
        [Display(Name = "Staff Number")]
        public int StaffNo { get; set; }

        [DisplayName("First name")]
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Family name")]
        [Required]
        [StringLength(50, ErrorMessage = "Family/last name cannot exceed 50 characters.")]
        [MinLength(2, ErrorMessage = "Family/last name must be at least 2 characters.")]
        public string FamilyName { get; set; } = string.Empty;

        [Display(Name = "Staff name")]
        public string? Fullname
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                    return FamilyName;

                return $"{FirstName} {FamilyName}";
            }
        }

        [DisplayName("Phone home")]
        [StringLength(30, ErrorMessage = "Home phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Home phone number must be at least 6 digits.")]
        public string? PhoneHome { get; set; } = string.Empty;

        [DisplayName("Mobile phone")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Mobile phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Mobile phone number must be at least 6 digits.")]
        public string? PhoneMobile { get; set; } = string.Empty;

        [DisplayName("Email")]
        [StringLength(256, ErrorMessage = "Email address cannot have more than 256 digits")]
        [MinLength(6, ErrorMessage = "An email address must be at least 6 digits.")]
        [Remote(action: "IsEmailAvailable", controller: "Staff", AdditionalFields = "staffId", ErrorMessage = "Email is already in use.")]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [Display(Name = "Contact detail")]
        public string? ContactDetail
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PhoneMobile))
                    return PhoneMobile;
                if (!string.IsNullOrWhiteSpace(PhoneHome))
                    return PhoneHome;

                return Email;
            }
        }

        public AddressViewModel Address { get; set; } = new(); // Embedded Address Object

        [DisplayName("IRD number")]
        [StringLength(12, ErrorMessage = "IRD number must have between 11 and 12 digits")]
        [MinLength(11, ErrorMessage = "IRD number must have between 11 and 12 digits")]
        public string? IrdNumber { get; set; } = string.Empty;

        [DisplayName("Bank account name")]
        [Column(TypeName = "varchar")]
        [StringLength(200, ErrorMessage = "Bank account must have between 3 and 200 letters")]
        [MinLength(3, ErrorMessage = "Bank account must have between 3 and 200 letters")]
        public string? BankAccountName { get; set; } = string.Empty;

        [DisplayName("Bank account number")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account number must have between 19 and 20 digits")]
        [MinLength(19, ErrorMessage = "Bank account number must have between 19 and 20 digits")]
        public string? BankAccountNumber { get; set; } = string.Empty;

        [DisplayName("Bank account particulars")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account particulars must have between 2 and 20 digits")]
        [MinLength(2, ErrorMessage = "Bank account particulars must have between 2 and 20 digits")]
        public string? BankAccountParticulars { get; set; } = string.Empty;

        [DisplayName("Bank account code")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account code must have between 2 and 20 digits")]
        [MinLength(2, ErrorMessage = "Bank account code must have between 2 and 20 digits")]
        public string? BankAccountCode { get; set; } = string.Empty;

        [DisplayName("Bank account reference")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account reference must have between 2 and 20 digits")]
        [MinLength(19, ErrorMessage = "Bank account reference must have between 2 and 20 digits")]
        public string? BankAccountReference { get; set; } = string.Empty;

        [DisplayName("Payroll number")]
        [StringLength(10)]
        public string? PayrollId { get; set; } = string.Empty;

        [DisplayName("Job title")]
        [StringLength(20)]
        public string? JobTitle { get; set; } = string.Empty;

        [DisplayName("Work role")]
        [Required]
        public WorkRole WorkRole { get; set; } // Admin, Supplier, Employee, Client

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        public string? ApplicationUserId { get; set; }
        public bool HasApplicationUser => !string.IsNullOrWhiteSpace(ApplicationUserId);
        
        [ScaffoldColumn(false)]
        public bool ShowPasswordSection { get; set; } = false;

        [DataType(DataType.Password)]
        public string? Password { get; set; } = string.Empty;

        public ChangePasswordViewModel ChangePassword { get; set; } = new();
    }
}
