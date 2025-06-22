// ReportService.cs
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using OfficeOpenXml;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Globalization;
using CleanMe.Domain.Interfaces;
using OfficeOpenXml.Style;
using System.Data;
using CleanMe.Application.DTOs;
using System.Drawing;


namespace CleanMe.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly ICompanyInfoService _companyInfoService;
        private readonly ISettingService _settingService;
        private readonly IAreaService _areaService;
        private readonly IStaffService _staffService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            ICompanyInfoService companyInfoService, 
            ISettingService settingService,
            IAreaService areaService,
            IStaffService staffService,
            IUnitOfWork unitOfWork,
            ILogger<ReportService> logger)
        {
            _companyInfoService = companyInfoService;
            _settingService = settingService;
            _areaService = areaService;
            _staffService = staffService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> GenerateScheduleExportToExcelAsync(ScheduleExportToExcelViewModel model)
        {
            // Adjust date range
            var firstSunday = model.DateFrom.Value;
            while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                firstSunday = firstSunday.AddDays(1);

            var lastSaturday = model.DateTo.Value;
            while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                lastSaturday = lastSaturday.AddDays(1);

            // Get output path
            var exportRoot = await _settingService.GetSettingValueAsync("ExcelExportPath");
            var folderName = $"{firstSunday:yyyy MMMM}";
            var fullPath = Path.Combine(exportRoot, folderName);
            Directory.CreateDirectory(fullPath);

            var weekStart = firstSunday.ToDateTime(TimeOnly.MinValue);
            var weekEnd = lastSaturday.ToDateTime(TimeOnly.MinValue);

            var area = _areaService.GetAreaViewModelByIdAsync(model.AreaId ?? 0).GetAwaiter().GetResult().Name;

            var baseName = model.DateRangeType == "Month"
                ? $"Schedule_{firstSunday:yyyy_MM}_Area_Cleaner.xlsx"
                : $"Schedule_{firstSunday:yyyy}_W{CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(weekStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday):D2}_to_W{CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(weekEnd, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday):D2}_Area_Cleaner.xlsx";

            var fileName = Path.Combine(fullPath, baseName);
            if (File.Exists(fileName))
            {
                var suffix = DateTime.Now.ToString("yyyyMMdd_HHmm");
                fileName = Path.Combine(fullPath, Path.GetFileNameWithoutExtension(baseName) + $"_{suffix}.xlsx");
            }

            // Fetch data from stored procedure
            var parameters = new
            {
                model.CleanerId,
                model.ClientId,
                model.RegionId,
                model.AreaId,
                model.AssetLocationId,
                model.AssetId,
                DateFrom = firstSunday,
                DateTo = lastSaturday
            };

            // Use Dapper to execute the stored procedure and get the data
            var data = await _unitOfWork.DapperRepository.QueryAsync<ScheduleReportRowDto>(
                "ScheduledCleaning",
                parameters,
                commandType: CommandType.StoredProcedure);

            // Group data by Cleaner
            var grouped = data.GroupBy(x => x.CleanerName).ToList();
            var company = await _companyInfoService.GetCompanyInfoViewModelAsync();

            using var package = new ExcelPackage();
            foreach (var group in grouped)
            {
                var ws = package.Workbook.Worksheets.Add(group.Key);

                // Header
                ws.Cells["A1:E2"].Merge = true;
                ws.Cells["A1"].Value = company.Name + Environment.NewLine + company.Phone + Environment.NewLine + company.Email;
                //ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //ws.Cells["A1"].Style.WrapText = true;

                ws.Cells["F1:H2"].Merge = true;
                ws.Cells["F1"].Value = company.Address.ToMultiLine();
                //ws.Cells["F1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //ws.Cells["F1"].Style.WrapText = true;

                ws.Cells["B1:D2"].Merge = true;
                ws.Cells["B1"].Value = group.First().CleanerAddress;
                ws.Cells["B1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells["B1"].Style.WrapText = true;

                ws.Cells["G1:I2"].Merge = true;
                ws.Cells["G1"].Value = $"Schedule for {firstSunday:MMMM yyyy}";

                // Generate all Sundays between DateFrom and DateTo (inclusive)
                List<DateTime> sundays = GetSundaysBetween(firstSunday, lastSaturday);

                // Create dynamic headers
                var headers = new List<string> { "Location", "MD ID", "Bank", "Freq" };
                headers.AddRange(sundays.Select(d => d.ToString("dd MMM")));

                int rowStart = 6;
                for (int i = 0; i < headers.Count; i++)
                    ws.Cells[rowStart, i + 1].Value = headers[i];

                // Data
                int row = rowStart + 1;
                foreach (var item in group)
                {
                    int col = 1;
                    ws.Cells[row, col++].Value = item.Date;
                    ws.Cells[row, col++].Value = item.Location;
                    ws.Cells[row, col++].Value = item.MdId;
                    ws.Cells[row, col++].Value = item.Frequency;

                    // Dynamically insert one value per Sunday (max 5)
                    for (int i = 0; i < sundays.Count && i < 5; i++)
                    {
                        var value = i switch
                        {
                            0 => item.Week1,
                            1 => item.Week2,
                            2 => item.Week3,
                            3 => item.Week4,
                            4 => item.Week5,
                            _ => ""
                        };

                        ws.Cells[row, col++].Value = value;
                    }

                    row++;
                }

                // Conditional formatting on Frequency (column 4)
                var range = ws.Cells[$"D{rowStart + 1}:D{row - 1}"];
                var red = ws.ConditionalFormatting.AddEqual(range);
                red.Formula = "\"W\"";
                red.Style.Fill.BackgroundColor.SetColor(Color.Red);

                var green = ws.ConditionalFormatting.AddEqual(range);
                green.Formula = "\"Q\"";
                green.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.PrinterSettings.RepeatRows = new ExcelAddress($"${rowStart}:${rowStart}");
            }

            package.SaveAs(new FileInfo(fileName));
            return fileName;
        }

        private List<DateTime> GetSundaysBetween(DateOnly from, DateOnly to)
        {
            var sundays = new List<DateTime>();
            var date = from.ToDateTime(TimeOnly.MinValue);
            var end = to.ToDateTime(TimeOnly.MinValue);

            while (date <= end)
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                    sundays.Add(date);

                date = date.AddDays(1);
            }

            return sundays;
        }
    }
}


//using CleanMe.Application.Helpers.Interfaces;
//using CleanMe.Application.Services.Interface;
//using Microsoft.AspNetCore.Mvc;
//using Syncfusion.XlsIO;
//using System;
//using System.Data;
//using System.IO;
//using Microsoft.Extensions.Logging;
//using CleanMe.Application.Services.Models;
//using CleanMe.Application.Helpers.Utilities;
//using System.Globalization;

//namespace CleanMe.Application.Services
//{
//    public class ReportService : IReportService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly ILogger<IReportsService> _logger;
//        private readonly Dictionary<int, char> _numberToColumn;

//        public ReportService(IUnitOfWork unitOfWork, ILogger<IReportsService> logger)
//        {
//            this._unitOfWork = unitOfWork;
//            this._logger = logger;
//            _numberToColumn = new Dictionary<int, char>()
//                        {
//                            { 1, 'A' },
//                            { 2, 'B' },
//                            { 3, 'C' },
//                            { 4, 'D' },
//                            { 5, 'E' },
//                            { 6, 'F' },
//                            { 7, 'G' },
//                            { 8, 'H' },
//                            { 9, 'I' },
//                            { 10, 'J' },
//                            { 11, 'K' },
//                            { 12, 'L' },
//                            { 13, 'M' },
//                            { 14, 'N' },
//                            { 15, 'O' },
//                            { 16, 'P' },
//                            { 17, 'Q' },
//                            { 18, 'R' },
//                            { 19, 'S' },
//                            { 20, 'T' },
//                            { 21, 'U' },
//                            { 22, 'V' },
//                            { 23, 'W' },
//                            { 24, 'X' },
//                            { 25, 'Y' },
//                            { 26, 'Z' }
//                        };
//        }

//        //public byte[] ExportScheduleToExcel(int StaffId, int Year, int Month)
//        public IEnumerable<string> ExportScheduleToExcel(ExportScheduleToExcelModel exportModel)
//        {
//            List<string> result = new List<string>();

//            try
//            {
//                Dictionary<string, string> parameters = new Dictionary<string, string>();
//                parameters.Add("staffId", exportModel.StaffId.ToString());
//                parameters.Add("year", exportModel.Year.ToString());
//                parameters.Add("month", exportModel.Month.ToString());
//                parameters.Add("areaId", "0");

//                // todo
//                // dbo.GetStaffAreas
//                // for each staffArea product report, named staff area.

//                DataTable reportHeader = _unitOfWork.SqlRepository.GetDataTableFromSql("dbo.GetStaffAreas", parameters);
//                foreach (DataRow row in reportHeader.Rows)
//                {
//                    int staffId = Convert.ToInt32(row["staffId"]);
//                    string staffName = row["StaffName"].ToString();
//                    string staffAddress = row["StaffAddress"].ToString();
//                    int areaId = Convert.ToInt32(row["areaId"]);
//                    string areaCode = row["AreaCode"].ToString();
//                    string areaName = row["AreaName"].ToString();

//                    staffName = FormatText.ToProperCase(staffName, " ");

//                    parameters["areaId"] = areaId.ToString();
//                    // update staff id with current row staff id
//                    parameters["staffId"] = staffId.ToString();

//                    // Produce Excel workbook for each staff for each area
//                    DataTable reportData = _unitOfWork.SqlRepository.GetDataTableFromSql("dbo.CleaningScheduleForMonth", parameters);
//                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", "Schedule.xltx");
//                    var fileName = string.Format("Schedule_{0}_{1}_{2}_{3}.xlsx", exportModel.Year.ToString(), exportModel.Month.ToString(), staffName, areaName.Replace(@"/", "-"));

//                    string folderName = string.Format("{0} {1}", exportModel.Year.ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(exportModel.Month));
//                    folderName = Path.Combine(Directory.GetCurrentDirectory(), "C:", "_tmp", folderName);
//                    if (!Directory.Exists(folderName))
//                    {
//                        Directory.CreateDirectory(folderName);
//                    }

//                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName);

//                    if (File.Exists(filePath))
//                    {
//                        File.Delete(filePath);
//                    }

//                    using (ExcelEngine excelEngine = new ExcelEngine())
//                    {
//                        IApplication application = excelEngine.Excel;
//                        IWorkbook workbook = application.Workbooks.Create(1);
//                        IWorksheet worksheet = workbook.Worksheets[0];

//                        // Add header
//                        IRange cellCompany = worksheet.Range["A1"];
//                        cellCompany.Text = exportModel.CompanyAndContactDetails;
//                        cellCompany.WrapText = true;
//                        cellCompany.AutofitRows();
//                        cellCompany.AutofitColumns();

//                        IRange cellAddress = worksheet.Range["G1"];
//                        cellAddress.Text = exportModel.Address;
//                        cellAddress.WrapText = true;
//                        cellAddress.AutofitRows();
//                        cellAddress.AutofitColumns();

//                        // Add data
//                        int firstDataRow = 3;
//                        worksheet.ImportDataTable(reportData, true, firstDataRow, 1);

//                        if (reportData.Rows.Count == 0)
//                        {
//                            string messageCellAddress = string.Format("A{0}", (firstDataRow + 2).ToString());
//                            worksheet.Range[messageCellAddress].Value = "No scheduled cleaning";
//                        }

//                        int lastRow = worksheet.UsedRange.LastRow;
//                        int lastCol = worksheet.UsedRange.LastColumn;
//                        _numberToColumn.TryGetValue(lastCol, out char lastColumn);

//                        string cellRangeAddress = string.Format("A1:{0}{1}", lastColumn, lastRow.ToString());
//                        worksheet.Range[cellRangeAddress].AutofitColumns();

//                        // Set footer with client code, area name, and page numbers
//                        IPageSetup pageSetup = worksheet.PageSetup;
//                        //pageSetup.LeftFooter = $"Client Code: {clientCode}\nArea: {areaName}";
//                        pageSetup.CenterFooter = $"Client Code: {areaCode}\nArea: {areaName}";
//                        pageSetup.RightFooter = "Page &P of &N";

//                        // Format columns
//                        int column = exportModel.StaffId == 0 ? 2 : 1;
//                        ApplyConditionalFormatting(worksheet, column, firstDataRow);

//                        // Save the Excel file to a memory stream
//                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
//                        {
//                            workbook.SaveAs(fileStream);
//                        }

//                        result.Add(filePath);
//                    }



//                    // Save the Excel file to a memory stream and then write to file
//                    //using (MemoryStream memoryStream = new MemoryStream())
//                    //{
//                    //    workbook.SaveAs(memoryStream);
//                    //    memoryStream.Position = 0; // Reset the position to the beginning of the stream

//                    //    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
//                    //    {
//                    //        memoryStream.CopyTo(fileStream);
//                    //    }
//                    //    //return memoryStream.ToArray();
//                    //    MemoryStream stream = new MemoryStream();
//                    //}

//                    //// Save the Excel file to a memory stream
//                    //using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
//                    //{
//                    //    workbook.SaveAs(fileStream);
//                    //}

//                    ////Save the Excel file to a memory stream
//                    //using (MemoryStream stream = new MemoryStream())
//                    //{
//                    //    workbook.SaveAs(stream);
//                    //    return stream.ToArray();
//                    //}
//                }
//                return result;
//            }
//            catch (Exception ex)
//            {
//                // Log the exception (you can use a logging framework here)
//                _logger.LogError(ex, "Error generating Excel file");
//                throw new InvalidOperationException("Error generating Excel file", ex);
//            }

//            return result;
//        }

//        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetMonthSelectListItems()
//        {
//            var months = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
//                    {
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "January", Value = "1" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "February", Value = "2" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "March", Value = "3" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "April", Value = "4" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "May", Value = "5" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "June", Value = "6" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "July", Value = "7" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "August", Value = "8" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "September", Value = "9" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "October", Value = "10" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "November", Value = "11" },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "December", Value = "12" }
//                    };

//            int currentMonth = DateTime.Now.Month;
//            int nextMonth = currentMonth == 12 ? 1 : currentMonth + 1; // Wrap around to January if the current month is December

//            months.ForEach(m => m.Selected = (m.Value == nextMonth.ToString()));
//            return months;
//        }

//        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetYearSelectListItems()
//        {
//            var currentYear = DateTime.Now.Year;

//            var years = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
//                    {
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = (currentYear - 1).ToString(), Value = (currentYear - 1).ToString() },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = currentYear.ToString(), Value = currentYear.ToString() },
//                        new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = (currentYear + 1).ToString(), Value = (currentYear + 1).ToString() }
//                    };

//            int currentMonth = DateTime.Now.Month;
//            int defaultYear = currentMonth == 12 ? currentYear + 1 : currentYear;

//            years.ForEach(y => y.Selected = (y.Value == defaultYear.ToString()));

//            return years;
//        }

//        private void ApplyConditionalFormatting(IWorksheet worksheet, int column, int firstDataRow)
//        {
//            _numberToColumn.TryGetValue(column, out char cellColumn);

//            int firstRow = firstDataRow + 1;
//            int lastRow = worksheet.UsedRange.LastRow;
//            // Format column 'MD ID' as text from row 2 to the last row
//            string cellRangeAddress = string.Format("{0}{1}:{0}{2}", cellColumn, firstRow.ToString(), lastRow);
//            worksheet.Range[cellRangeAddress].NumberFormat = "@";

//            // Apply conditional formatting to column C from row 2 to the last row
//            column = column + 2;
//            _numberToColumn.TryGetValue(column, out cellColumn);
//            cellRangeAddress = string.Format("{0}{1}:{0}{2 }", cellColumn, firstRow.ToString(), lastRow);
//            IConditionalFormats conditionalFormats = worksheet.Range[cellRangeAddress].ConditionalFormats;
//            IConditionalFormat condition1 = conditionalFormats.AddCondition();
//            condition1.FormatType = ExcelCFType.CellValue;
//            condition1.Operator = ExcelComparisonOperator.Equal;
//            condition1.FirstFormula = "\"M\"";
//            condition1.BackColorRGB = Syncfusion.Drawing.Color.Red;

//            IConditionalFormat condition2 = conditionalFormats.AddCondition();
//            condition2.FormatType = ExcelCFType.CellValue;
//            condition2.Operator = ExcelComparisonOperator.Equal;
//            condition2.FirstFormula = "\"Q\"";
//            condition2.BackColorRGB = Syncfusion.Drawing.Color.Green;
//        }
//    }
//}
