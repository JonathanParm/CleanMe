using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CleanMe.Domain.Common
{
    public class Address
    {
        [DisplayName("Line 1")]
        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(20, ErrorMessage = "An address line cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "An address line must be at least 2 characters.")]
        public string Line1 { get; set; } = string.Empty;

        [DisplayName("Line 2")]
        [Column(TypeName = "varchar(100)")]
        [StringLength(20, ErrorMessage = "An address line cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "An address line must be at least 2 characters.")]
        public string? Line2 { get; set; }


        [DisplayName("Suburb")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(20, ErrorMessage = "A suburb name cannot exceed 50 characters.")]
        [MinLength(2, ErrorMessage = "A suburb name must be at least 2 characters.")]
        public string? Suburb { get; set; }

        [DisplayName("Town/City")]
        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(20, ErrorMessage = "A town/city name cannot exceed 100 characters.")]
        [MinLength(2, ErrorMessage = "A town/city name must be at least 2 characters.")]
        public string TownOrCity { get; set; } = string.Empty;

        [DisplayName("Postcode")]
        [Required]
        [Column(TypeName = "varchar(10)")]
        [StringLength(20, ErrorMessage = "A postcode cannot exceed 10 characters.")]
        [MinLength(4, ErrorMessage = "A postcode must be at least 4 characters.")]
        public string Postcode { get; set; } = string.Empty;

        public Address() { }

        public Address(string line1, string? line2, string? suburb, string townOrCity, string postcode)
        {
            Line1 = line1;
            Line2 = line2;
            Suburb = suburb;
            TownOrCity = townOrCity;
            Postcode = postcode;
        }

        // Returns a single-line formatted address
        public override string ToString()
        {
            return $"{Line1}{(string.IsNullOrWhiteSpace(Line2) ? "" : ", " + Line2)} {(string.IsNullOrWhiteSpace(Suburb) ? "" : ", " + Suburb)}, {TownOrCity}, {Postcode}";
        }

        // Returns a multi-line formatted address
        public string ToMultiLine()
        {
            return $"{Line1}\n{(string.IsNullOrWhiteSpace(Line2) ? "" : Line2 + "\n")}{(string.IsNullOrWhiteSpace(Suburb) ? "" : Suburb + "\n")}{TownOrCity} {Postcode}";
        }
    }
}