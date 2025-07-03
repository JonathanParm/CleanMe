using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanMe.Application.DTOs;
using CleanMe.Application.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMe.Application.Interfaces
{
    public interface IReportService
    {
        Task<string> GenerateExportStaffScheduleToExcelAsync(ExportStaffScheduleToExcelViewModel model);

        Task<string> GenerateExportClientScheduleToExcelAsync(ExportClientScheduleToExcelViewModel model);

        //IEnumerable<string> ExportScheduleToExcel(ExportScheduleToExcelDto exportModel);
        //string ExportScheduleToExcel(ExportScheduleToExcelModel exportModel);
        //byte[] ExportScheduleToExcel(int StaffId, int Year, int Month);
        //IEnumerable<SelectListItem> GetYearSelectListItems();
        //IEnumerable<SelectListItem> GetMonthSelectListItems();
    }
}
