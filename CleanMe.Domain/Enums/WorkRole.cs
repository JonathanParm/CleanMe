using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Enums
{
    public enum WorkRole
    {
    [Display(Name = "Administrator")]
        Admin,
    [Display(Name = "Cleaner")]
        Cleaner,
    [Display(Name = "Contractor")]
        Contractor,
    [Display(Name = "Client")]
        Client
    }
}