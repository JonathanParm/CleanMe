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
    public class AssetLocationWithAddressDto
    {
        public int assetLocationId { get; set; }
        public string Description { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string? AddressSuburb { get; set; }
        public string AddressTownOrCity { get; set; } = string.Empty;
        public string AddressPostcode { get; set; } = string.Empty;
        public int SequenceOrder { get; set; } = 0;
        public int SeqNo { get; set; } = 0;
        public string ReportCode { get; set; }
        public int AccNo { get; set; }
        public int areaId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime AddedAt { get; set; }
        public string AddedById { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedById { get; set; }
    }
}
