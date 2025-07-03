using CleanMe.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CleanMe.Application.ViewModels
{
    public class AmendmentViewModel
    {
        public int? amendmentId { get; set; }
        public bool IsEdit => amendmentId.HasValue;

        [Display(Name = "Name of source")]
        public string? AmendmentSourceName { get; set; }

        public int? amendmentTypeId { get; set; } = null;
        public int? areaId { get; set; } = null;
        public int? assetId { get; set; } = null;
        public int? assetLocationId { get; set; } = null;
        public int? cleanFrequencyId { get; set; } = null;
        public int? clientId { get; set; } = null;
        public int? itemCodeId { get; set; } = null;
        public int? staffId { get; set; } = null;

        public string? AmendmentTypeName { get; set; } = null;
        public string? AreaName { get; set; } = null;
        public string? AssetName { get; set; } = null;
        public string? AssetLocationName { get; set; } = null;
        public string? CleanFrequencyName { get; set; } = null;
        public string? ClientName { get; set; } = null;
        public string? ItemCodeName { get; set; } = null;
        public string? StaffName { get; set; } = null;

        [DisplayName("Rate")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Rate { get; set; } = null;

        [Display(Name = "Access to asset")]
        public string? Access { get; set; } = null;

        [Display(Name = "Is Accessable")]
        public bool IsAccessable { get; set; } = true;

        [DisplayName("Start on")]
        [DataType(DataType.DateTime)]
        public DateTime? StartOn { get; set; }

        [DisplayName("Finish on")]
        [DataType(DataType.DateTime)]
        public DateTime? FinishOn { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(2000, MinimumLength = 2, ErrorMessage = "Comment about amendment to schedule must have between 2 and 2000 letters")]
        [Display(Name = "Comment about amendment")]
        public string? Comment { get; set; }

        [DisplayName("Invoiced")]
        [DataType(DataType.DateTime)]
        public DateTime? InvoicedOn { get; set; }

        public string? AmendmentSummary
        {
            get
            {
                var sb = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(AmendmentSourceName))
                    sb.AppendLine($"Source name is {AmendmentSourceName}");

                if (clientId != null)
                    sb.AppendLine($"Client is {ClientName}");

                if (areaId != null)
                    sb.AppendLine($"Area name is {AreaName}");

                if (assetLocationId != null)
                    sb.AppendLine($"Asset location is {AssetLocationName}");

                if (assetId != null)
                    sb.AppendLine($"Asset is {AssetName}");

                if (cleanFrequencyId != null)
                    sb.AppendLine($"Clean frequency is {CleanFrequencyName}");

                if (staffId != null)
                    sb.AppendLine($"Cleaner is {StaffName}");

                if (Rate.HasValue)
                    sb.AppendLine($"Cleaning rate is {Rate.Value:C}");

                if (StartOn.HasValue)
                    sb.AppendLine($"Starts on {StartOn.Value:d MMM yyyy}");

                if (FinishOn.HasValue)
                    sb.AppendLine($"Finishes on {FinishOn.Value:d MMM yyyy}");

                return sb.ToString().Trim();
            }
        }
    }
}