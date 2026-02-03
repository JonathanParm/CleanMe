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
        [Display(Name = "ID")]
        public int clientId { get; set; }

        [Required]
        [Display(Name = "Client")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Client name must have between 3 and 100 letters")]
        public string ClientName { get; set; }

        [Display(Name = "Brand")]
        [Column(TypeName = "varchar")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Client brand must have between 2 and 10 letters")]
        public string? Brand { get; set; }

        [Display(Name = "DR Accs")]
        public int AccNo { get; set; }

        [Display(Name = "Reference")]
        [Column(TypeName = "varchar")]
        [StringLength(50, ErrorMessage = "Client reference cannot have more than 50 letters")]
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
