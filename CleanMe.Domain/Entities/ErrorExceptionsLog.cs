using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CleanMe.Domain.Entities
{
    public class ErrorExceptionsLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int logId { get; set; }

        [ForeignKey("ApplicationUser")]
        [Column(TypeName = "NVARCHAR(450)")]
        public string? ApplicationUserId { get; set; }

        [Required]
        [Display(Name = "Occurred at")]
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Message")]
        [Required]
        [Column(TypeName = "NVARCHAR(1000)")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Stack trace")]
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string StackTrace { get; set; } = string.Empty;

        [Display(Name = "Message")]
        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string Source { get; set; } = string.Empty;

        [Display(Name = "Request path")]
        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string RequestPath { get; set; } = string.Empty;
    }
}