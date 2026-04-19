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

        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar(50)")]
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

        [Column(TypeName = "varchar(30)")]
        public string? PhoneHome { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string? PhoneMobile { get; set; }

        [Column(TypeName = "varchar(256)")]
        public string? Email { get; set; }

        public Address Address { get; set; } = new Address(); // Embedded Address Object

        [Column(TypeName = "varchar(12)")]
        public string? IrdNumber { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? BankAccountName { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? BankAccountNumber { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? BankAccountParticulars { get; set; }

        [Display(Name = "Bank account code")]
        [Column(TypeName = "varchar")]
        [StringLength(20, ErrorMessage = "Bank account code must have between 2 and 20 letters")]
        [MinLength(2, ErrorMessage = "Bank account name must have between 2 and 20 letters")]
        public string? BankAccountCode { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? BankAccountReference { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? PayrollId { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? JobTitle { get; set; }

        [Column(TypeName = "varchar(50)")]
        public WorkRole WorkRole { get; set; } // Admin, Contractor, Cleaner

        public bool IsActive { get; set; } = true;

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; } = false;

        public DateTime AddedAt { get; set; }

        [Column(TypeName = "NVARCHAR(450)")]
        public string AddedById { get; set; }

        [Display(Name = "Updated at")]
        public DateTime UpdatedAt { get; set; }

        [Column(TypeName = "NVARCHAR(450)")]
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
