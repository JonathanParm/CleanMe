using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    [Table("ItemCodeRates")]
    public class ItemCodeRate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int itemCodeRateId { get; set; }

        [Required]
        [Display(Name = "Name")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Item code rate name must have between 3 and 100 letters")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Item code rate description must have between 2 and 100 letters")]
        public string? Description { get; set; }

        [ForeignKey("ItemCode")]
        [Display(Name = "Item code")]
        public int itemCodeId { get; set; }

        // Navigation property
        [Display(Name = "Item code")]
        [ForeignKey(nameof(itemCodeId))]
        public ItemCode? ItemCode { get; set; }

        [ForeignKey("Frequency")]
        [Display(Name = "Clean Frequency")]
        public int cleanFrequencyId { get; set; }

        // Navigation property
        [Display(Name = "Clean Frequency")]
        [ForeignKey(nameof(cleanFrequencyId))]
        public CleanFrequency? CleanFrequency { get; set; }

        [ForeignKey("Client")]
        [Display(Name = "Client")]
        public int clientId { get; set; }

        // Navigation property
        [Display(Name = "Client")]
        [ForeignKey(nameof(clientId))]
        public Client? Client { get; set; }

        [Required]
        [Display(Name = "Rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Rate { get; set; }

        [Required]
        [Display(Name = "Default rate")]
        public bool IsDefault { get; set; } = false;

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
    }
}