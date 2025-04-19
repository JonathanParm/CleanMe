using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Entities
{
    [Table("ClientContacts")]
    public class ClientContact
    {
        [Key]
        [DisplayName("Contact")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int clientContactId { get; set; }

        [ForeignKey("ApplicationUser")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string? ApplicationUserId { get; set; }

        [DisplayName("First name")]
        [Required(ErrorMessage = "Must have first name")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must have between 2 and 50 letters")]
        public string FirstName { get; set; }

        [DisplayName("Family name")]
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

        [DisplayName("Job title")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string? JobTitle { get; set; }

        [DisplayName("Mobile phone")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(30, ErrorMessage = "Mobile phone number cannot have more than 30 digits")]
        [MinLength(6, ErrorMessage = "Mobile phone number must be at least 6 digits.")]
        public string? PhoneMobile { get; set; }

        [DisplayName("Email")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(256, ErrorMessage = "Email address must have at least 6 characters")]
        [MinLength(6, ErrorMessage = "An email address must be at least 6 digits.")]
        public string? Email { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;

        //// Foreign key property linking to the parent
        //[ForeignKey("Client")]
        //[DisplayName("Client")]
        //public int ClientId { get; set; }
        //// Navigation property representing the parent
        //[DisplayName("Client")]
        //public Client? Client { get; set; }

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

        // Foreign Key
        [ForeignKey("Client")]
        [DisplayName("Client")]
        public int clientId { get; set; }

        // Navigation property
        public Client Client { get; set; } = null!;
    }
}
