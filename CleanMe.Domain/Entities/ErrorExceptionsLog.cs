using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
        [DisplayName("Occurred at")]
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        [DisplayName("Message")]
        [Required]
        [Column(TypeName = "NVARCHAR(1000)")]
        public string Message { get; set; } = string.Empty;

        [DisplayName("Stack trace")]
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string StackTrace { get; set; } = string.Empty;

        [DisplayName("Message")]
        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string Source { get; set; } = string.Empty;

        [DisplayName("Request path")]
        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string RequestPath { get; set; } = string.Empty;
    }
}