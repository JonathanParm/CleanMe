using Microsoft.AspNetCore.Identity;

namespace CleanMe.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        // No direct link to Staff to avoid circular dependency.

        // add extra fields to the identity user
    }
}
