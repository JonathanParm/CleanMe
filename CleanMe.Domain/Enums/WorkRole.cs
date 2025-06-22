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
    [Display(Name = "Employee")]
        Employee,
    [Display(Name = "Supplier")]
        Supplier,
    [Display(Name = "Client")]
        Client
    }
}