using System.ComponentModel.DataAnnotations;

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