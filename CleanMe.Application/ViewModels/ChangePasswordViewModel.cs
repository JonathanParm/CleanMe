using System.ComponentModel.DataAnnotations;

namespace CleanMe.Application.ViewModels
{
    public class ChangePasswordViewModel
    {
        [ScaffoldColumn(false)] // Prevent Razor auto-generation
        public string? UserId { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}