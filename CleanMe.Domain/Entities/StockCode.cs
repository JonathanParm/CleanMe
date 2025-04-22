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
    [Table("StockCodes")]
    public class StockCode
    {
        [Key]
        [DisplayName("Stock code")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int stockCodeId { get; set; }

        [Required(ErrorMessage = "Must have code")]
        [DisplayName("Code")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Code must have between 3 and 50 letters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Must have description")]
        [DisplayName("Description")]
        [Column(TypeName = "VARCHAR")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Description must have between 3 and 500 letters")]
        public string Description { get; set; }

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

        // Navigation property
        public ICollection<AssetType> AssetTypes { get; set; } = new List<AssetType>();
    }
}
