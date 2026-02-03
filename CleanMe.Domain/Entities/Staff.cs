using CleanMe.Domain.Common;
using CleanMe.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Domain.Entities
{
    public class Staff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int staffId { get; set; }

        [ForeignKey("ApplicationUser")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string? ApplicationUserId { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "Must have first name")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Family name")]
        [Required(ErrorMessage = "Must have family name")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50, ErrorMessage = "Family/last name cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "Family/last name must be at least 2 characters.")]
        public string FamilyName { get; set; }

        [Display(Name = "Staff name")]
        public string? Fullname
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                    return FamilyName;

                return $"{FamilyName}, {FirstName}";
            }
        }

        [Display(Name = "Phone home")]
        [Column(TypeName = "varchar")]
        [StringLength(30, ErrorMessage = "Home phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Home phone number must be at least 6 digits.")]
        public string? PhoneHome { get; set; }

        [Display(Name = "Mobile phone")]
        [Column(TypeName = "varchar")]
        [StringLength(30,ErrorMessage = "Mobile phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Mobile phone number must be at least 6 digits.")]
        public string? PhoneMobile { get; set; }

        [Display(Name = "Email")]
        [Column(TypeName = "varchar")]
        [StringLength(256, ErrorMessage = "Email address must have at least 6 characters")]
        [MinLength(6, ErrorMessage = "An email address must be at least 6 digits.")]
        public string? Email { get; set; }

        public Address Address { get; set; } = new Address(); // Embedded Address Object

        [Display(Name = "IRD number")]
        [Column(TypeName = "varchar")]
        [StringLength(12, ErrorMessage = "IRD number must have between 11 and 12 digits")]
        [MinLength(11, ErrorMessage = "IRD number must have between 11 and 12 digits")]
        public string? IrdNumber { get; set; }

        [Display(Name = "Bank account name")]
        [Column(TypeName = "varchar")]
        [StringLength(200, ErrorMessage = "Bank account name must have between 3 and 200 letters")]
        [MinLength(19, ErrorMessage = "Bank account name must have between 3 and 200 letters")]
        public string? BankAccountName { get; set; }

        [Display(Name = "Bank account number")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account number must have between 19 and 20 digits")]
        [MinLength(19, ErrorMessage = "Bank account must have between 19 and 20 digits")]
        public string? BankAccountNumber { get; set; }

        [Display(Name = "Bank account particulars")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account particulars must have between 2 and 20 letters")]
        [MinLength(2, ErrorMessage = "Bank account particulars must have between 2 and 20 letters")]
        public string? BankAccountParticulars { get; set; }

        [Display(Name = "Bank account code")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account code must have between 2 and 20 letters")]
        [MinLength(2, ErrorMessage = "Bank account name must have between 2 and 20 letters")]
        public string? BankAccountCode { get; set; }

        [Display(Name = "Bank account reference")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account name must have between 2 and 20 letters")]
        [MinLength(2, ErrorMessage = "Bank account name must have between 2 and 20 letters")]
        public string? BankAccountReference { get; set; }

        [Display(Name = "Payroll number")]
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        public string? PayrollId { get; set; }

        [Display(Name = "Job title")]
        [Column(TypeName = "varchar")]
        [StringLength(20)]
        public string? JobTitle { get; set; }

        [Display(Name = "Work role")]
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(20)]
        public WorkRole WorkRole { get; set; } // Admin, Contractor, Cleaner

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Display(Name = "Added at")]
        public DateTime AddedAt { get; set; }

        [Required]
        [Display(Name = "Added by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string AddedById { get; set; }

        [Required]
        [Display(Name = "Updated at")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [Display(Name = "Updated by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string UpdatedById { get; set; }

        // Default constructor for Entity Framework
        public Staff()
        {
            FirstName = string.Empty;
            FamilyName = string.Empty;
            WorkRole = WorkRole.Employee;
        }

        // Constructor to ensure required properties are set
        public Staff(string firstName, string familyName, string workRole)
        {
            FirstName = firstName;
            FamilyName = familyName;
            WorkRole = WorkRole.Employee;
        }

        // Navigation property
        public ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();
    }
}
