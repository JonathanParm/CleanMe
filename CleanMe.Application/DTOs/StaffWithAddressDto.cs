using CleanMe.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanMe.Domain.Enums;

namespace CleanMe.Application.DTOs
{
    public class StaffWithAddressDto
    {
        public int staffId { get; set; }
        public string? ApplicationUserId { get; set; }
        public int StaffNo { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string? PhoneHome { get; set; }
        public string? PhoneMobile { get; set; }
        public string? Email { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string? AddressSuburb { get; set; }
        public string AddressTownOrCity { get; set; } = string.Empty;
        public string AddressPostcode { get; set; } = string.Empty;
        public string? IrdNumber { get; set; }
        public string? BankAccount { get; set; }
        public string? PayrollId { get; set; }
        public string? JobTitle { get; set; }
        public WorkRole WorkRole { get; set; } // Admin, Contractor, Cleaner
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime AddedAt { get; set; }
        public string AddedById { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedById { get; set; }
    }
}
