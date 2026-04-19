using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CleanMe.Domain.Common;

namespace CleanMe.Domain.Entities
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int clientId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string ClientName { get; set; }

        [Display(Name = "Brand")]
        [Column(TypeName = "varchar(10)")]
        public string? Brand { get; set; }

        public int AccNo { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? Reference { get; set; }

        //// Navigation property representing the collection of children
        //public ICollection<ClientContact>? Contacts { get; set; }
        //public ICollection<Asset>? Assets { get; set; }
        //public ICollection<ServiceRate>? ServiceRates { get; set; }

        public Address Address { get; set; } = new Address(); // Embedded Address Object

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
        [Display(Name = "Created by")]
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

        // Navigation property
        public ICollection<ClientContact> ClientContacts { get; set; } = new List<ClientContact>();

        // Navigation property
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();

        // Navigation property
        public ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();

        // Navigation property
        public ICollection<ItemCodeRate> AssetTypeRates { get; set; } = new List<ItemCodeRate>();
    }
}
