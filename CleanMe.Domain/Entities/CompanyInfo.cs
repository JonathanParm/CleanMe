using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CleanMe.Domain.Common;

namespace CleanMe.Domain.Entities
{
    [Table("CompanyInfo")]
    public class CompanyInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "ID")]
        public int companyInfoId { get; set; }

        [Required(ErrorMessage = "Must have company name")]
        [Display(Name = "Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Company name must have between 3 and 100 letters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Must have phone number")]
        [Display(Name = "Phone")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Phone number must have between 7 and 20 letters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Must have email address")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Email address must have between 3 and 100 letters")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        public Address Address { get; set; } = new Address(); // Embedded Address Object

        [Required]
        [Display(Name = "Updated at")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        [Display(Name = "Updated by")]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(450)]
        public string UpdatedById { get; set; }
    }
}
