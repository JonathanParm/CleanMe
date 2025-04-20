using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        // No direct link to Staff to avoid circular dependency.

        // add extra fields to the identity user
    }
}
