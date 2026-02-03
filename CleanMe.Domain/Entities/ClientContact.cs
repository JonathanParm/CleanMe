using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    [Table("ClientContacts")]
    public class ClientContact
    {
        [Key]
        [Display(Name = "Contact")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int clientContactId { get; set; }

        [ForeignKey("ApplicationUser")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string? ApplicationUserId { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "Must have first name")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must have between 2 and 50 letters")]
        public string FirstName { get; set; }

        [Display(Name = "Family name")]
        [Required(ErrorMessage = "Must have family name")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Family name must have between 2 and 50 letters")]
        public string FamilyName { get; set; }

        [Display(Name = "Contact name")]
        public string? ContactName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                    return FamilyName;

                return $"{FamilyName}, {FirstName}";
            }
        }

        [Display(Name = "Job title")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string? JobTitle { get; set; }

        [Display(Name = "Mobile phone")]
        [Column(TypeName = "varchar")]
        [StringLength(30, ErrorMessage = "Mobile phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Mobile phone number must be at least 6 digits.")]
        public string? PhoneMobile { get; set; }

        [Display(Name = "Email")]
        [Column(TypeName = "varchar")]
        [StringLength(256, ErrorMessage = "Email address must have at least 6 characters")]
        [MinLength(6, ErrorMessage = "An email address must be at least 6 digits.")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        //// Foreign key property linking to the parent
        //[ForeignKey("Client")]
        //[Display(Name = "Client")]
        //public int ClientId { get; set; }
        //// Navigation property representing the parent
        //[Display(Name = "Client")]
        //public Client? Client { get; set; }

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

        // Foreign Key
        [ForeignKey("Client")]
        [Display(Name = "Client")]
        public int clientId { get; set; }

        // Navigation property
        [Display(Name = "Client")]
        [ForeignKey(nameof(clientId))]
        public virtual Client? Client { get; set; }
    }
}
