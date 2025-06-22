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
    [Table("ItemCodeRates")]
    public class ItemCodeRate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int itemCodeRateId { get; set; }

        [Required]
        [DisplayName("Name")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Item code rate name must have between 3 and 100 letters")]
        public string Name { get; set; }

        [DisplayName("Description")]
        [Column(TypeName = "varchar")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Item code rate description must have between 2 and 100 letters")]
        public string? Description { get; set; }

        [ForeignKey("ItemCode")]
        [DisplayName("Item code")]
        public int itemCodeId { get; set; }

        // Navigation property
        [DisplayName("Item code")]
        [ForeignKey(nameof(itemCodeId))]
        public ItemCode? ItemCode { get; set; }

        [ForeignKey("Frequency")]
        [DisplayName("Clean Frequency")]
        public int cleanFrequencyId { get; set; }

        // Navigation property
        [DisplayName("Clean Frequency")]
        [ForeignKey(nameof(cleanFrequencyId))]
        public CleanFrequency? CleanFrequency { get; set; }

        [ForeignKey("Client")]
        [DisplayName("Client")]
        public int clientId { get; set; }

        // Navigation property
        [DisplayName("Client")]
        [ForeignKey(nameof(clientId))]
        public Client? Client { get; set; }

        [Required]
        [DisplayName("Rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Rate { get; set; }

        [Required]
        [DisplayName("Default rate")]
        public bool IsDefault { get; set; } = false;

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
    }
}