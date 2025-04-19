using CleanMe.Domain.Common;
using CleanMe.Domain.Enums;
using System.ComponentModel;
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

        [Required]
        [RegularExpression("[1-9]", ErrorMessage = "Staff number cannot be zero.")]
        [Display(Name = "Staff Number")]
        public int StaffNo { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "Must have first name")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        public string FirstName { get; set; }

        [DisplayName("Family name")]
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

        [DisplayName("Phone home")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30, ErrorMessage = "Home phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Home phone number must be at least 6 digits.")]
        public string? PhoneHome { get; set; }

        [DisplayName("Mobile phone")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30,ErrorMessage = "Mobile phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Mobile phone number must be at least 6 digits.")]
        public string? PhoneMobile { get; set; }

        [DisplayName("Email")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(256, ErrorMessage = "Email address must have at least 6 characters")]
        [MinLength(6, ErrorMessage = "An email address must be at least 6 digits.")]
        public string? Email { get; set; }

        public Address Address { get; set; } = new Address(); // Embedded Address Object

        [DisplayName("IRD number")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(12, ErrorMessage = "IRD number must have between 11 and 12 digits")]
        [MinLength(11, ErrorMessage = "IRD number must have between 11 and 12 letters")]
        public string? IrdNumber { get; set; }

        [DisplayName("Bank account")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20, ErrorMessage = "Bank account must have between 19 and 20 digits")]
        [MinLength(19, ErrorMessage = "Bank account must have between 19 and 20 digits")]
        public string? BankAccount { get; set; }

        [DisplayName("Payroll number")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string? PayrollId { get; set; }

        [DisplayName("Job title")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string? JobTitle { get; set; }

        [DisplayName("Work role")]
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public WorkRole WorkRole { get; set; } // Admin, Contractor, Cleaner

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [DisplayName("Deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [DisplayName("Added at")]
        public DateTime AddedAt { get; set; }

        [Required]
        [DisplayName("Added by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string AddedById { get; set; }

        [Required]
        [DisplayName("Updated at")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [DisplayName("Updated by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string UpdatedById { get; set; }

        // Default constructor for Entity Framework
        public Staff()
        {
            FirstName = string.Empty;
            FamilyName = string.Empty;
            WorkRole = WorkRole.Cleaner;
        }

        // Constructor to ensure required properties are set
        public Staff(int staffNo, string firstName, string familyName, string workRole)
        {
            StaffNo = staffNo;
            FirstName = firstName;
            FamilyName = familyName;
            WorkRole = WorkRole.Cleaner;
        }
    }
}
