using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IReportService
    {
        Task<(string FolderPath, string FileName)> GenerateExportStaffScheduleToExcelAsync(ExportStaffScheduleToExcelViewModel model);

        Task<(string FolderPath, string FileName)> GenerateExportClientScheduleToExcelAsync(ExportClientScheduleToExcelViewModel model);

        //IEnumerable<string> ExportScheduleToExcel(ExportScheduleToExcelDto exportModel);
        //string ExportScheduleToExcel(ExportScheduleToExcelModel exportModel);
        //byte[] ExportScheduleToExcel(int StaffId, int Year, int Month);
        //IEnumerable<SelectListItem> GetYearSelectListItems();
        //IEnumerable<SelectListItem> GetMonthSelectListItems();
    }
}
