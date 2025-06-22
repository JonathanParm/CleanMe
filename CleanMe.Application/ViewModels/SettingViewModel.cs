using CleanMe.Domain.Common;
using CleanMe.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanMe.Application.ViewModels
{
    public class SettingViewModel
    {
        public int settingId { get; set; }

        [Required(ErrorMessage = "Must have Export Excel folder path")]
        [DisplayName("Export to Excel folder")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Export to Excel folder path must have between 3 and 100 letters")]
        public string ExcelExportPath { get; set; }
    }
}
