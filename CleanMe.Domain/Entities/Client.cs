using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanMe.Domain.Common;

namespace CleanMe.Domain.Entities
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int clientId { get; set; }

        [Required]
        [DisplayName("Name")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100, ErrorMessage = "Client name must have between 3 and 100 letters")]
        public string Name { get; set; }

        [DisplayName("Brand")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Client brand must have between 2 and 10 letters")]
        public string? Brand { get; set; }

        [DisplayName("DR Accs")]
        public int AccNo { get; set; }

        //// Navigation property representing the collection of children
        //public ICollection<ClientContact>? Contacts { get; set; }
        //public ICollection<Asset>? Assets { get; set; }
        //public ICollection<ServiceRate>? ServiceRates { get; set; }

        public Address Address { get; set; } = new Address(); // Embedded Address Object

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
        [DisplayName("Created by")]
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

        // Navigation property
        public ICollection<ClientContact> ClientContacts { get; set; } = new List<ClientContact>();
    }
}
