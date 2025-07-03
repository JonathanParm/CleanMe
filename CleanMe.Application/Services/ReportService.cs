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
using Microsoft.AspNetCore.Http;


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

        public async Task<string> GenerateExportClientScheduleToExcelAsync(ExportClientScheduleToExcelViewModel model)
        {
            _logger.LogInformation("Generate Export client schedule to Excel Async.");
            try
            {

                // Adjust DateFrom to first Sunday of month, and DateTo to following Saturday last day of month if needed
                var firstSunday = DateTime.Parse(model.DateFrom.ToString());
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(1);

                var lastSaturday = DateTime.Parse(model.DateTo.ToString());
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);

                model.DateFrom = firstSunday;
                model.DateTo = lastSaturday;

                var companyInfo = await _companyInfoService.GetCompanyInfoViewModelAsync();
                var clientSchedules = await _unitOfWork.DapperRepository.QueryAsync<ExportClientScheduleToExcelDto>(
                "ExportClientScheduleToExcel",
                new
                {
                    model.DateFrom,
                    model.DateTo,
                    ClientId = model.ClientId ?? 0,
                },
                commandType: CommandType.StoredProcedure);

                var outputPath = await _settingService.GetSettingValueAsync("ExcelExportPath");
                if (string.IsNullOrWhiteSpace(outputPath))
                    throw new Exception("Excel export path is not configured.");

                var totalDays = (lastSaturday - firstSunday).Days;
                var sundays = Enumerable.Range(0, totalDays + 1)
                    .Select(d => firstSunday.AddDays(d))
                    .Where(d => d.DayOfWeek == DayOfWeek.Sunday)
                    .Select(d => DateOnly.FromDateTime(d))
                    .ToList();

                var groupedByClient = clientSchedules
                        .GroupBy(x => new { x.clientId, x.ClientName })
                        .OrderBy(g => g.Key.ClientName)
                        .ThenBy(g => g.Key.clientId);

                foreach (var clientGroup in groupedByClient)
                {
                    var clientName = clientGroup.Key.ClientName;
                    var monthFolderName = model.DateFrom.ToString();
                    var parsedDate = DateTime.Parse(monthFolderName);
                    var folder = Path.Combine(outputPath, parsedDate.ToString("yyyy MM"));
                    Directory.CreateDirectory(folder);

                    var baseFileName = $"Schedule {model.DateFrom:yyyy_MM} {clientName}.xlsx";
                    var fullPath = Path.Combine(folder, baseFileName);
                    if (File.Exists(fullPath))
                    {
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
                        baseFileName = $"Schedule {model.DateFrom:yyyy-MM} {clientName} {timestamp}.xlsx";
                        fullPath = Path.Combine(folder, baseFileName);
                    }

                    using var package = new ExcelPackage();
                    var ws = package.Workbook.Worksheets.Add("Schedule");

                    int row = 1;

                    // Company info: Name, phone, email (left), Address (right)
                    ws.Cells[row, 1, row, 4].Merge = true;
                    ws.Cells[row, 1].Value = $"{companyInfo.Name}\nPhone: {companyInfo.Phone}\nEmail: {companyInfo.Email}";

                    ws.Cells[row, 6, row, 8].Merge = true;
                    ws.Cells[row, 6].Value = companyInfo.Address;

                    row += 4;

                    // Client block left
                    ws.Cells[row, 1, row + 2, 4].Merge = true;
                    ws.Cells[row, 1].Value = clientGroup.First().ClientAddress;
                    ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    // Schedule title right
                    ws.Cells[row + 1, 6].Value = $"Schedule for {model.DateFrom:MMMM yyyy}";
                    ws.Cells[row + 1, 6].Style.Font.Bold = true;

                    row += 4;

                    int headerRow = 4;
                    int sundayRow = headerRow - 1;

                    // Fixed headers
                    var fixedHeaders = new List<string> { "Freq", "Wk1", "Wk2", "Wk3", "Wk4", "Wk5", "Type", "Client Ref.", "MD ID", "Region", "Area", "Location" };

                    // Sunday headers (only go above Wk1–Wk5, which are the last 5 columns)
                    var sundayHeaders = sundays.Select(d => d.ToString("d MMM")).ToList();

                    // Write Sunday row (row 3) starting at column 5 (above Wk1–Wk5)
                    for (int i = 0; i < sundayHeaders.Count; i++)
                    {
                        ws.Cells[sundayRow, i + 2].Value = sundayHeaders[i];
                        ws.Cells[sundayRow, i + 2].Style.Font.Bold = true;
                    }

                    // Write Column Labels row (row 4)
                    //var allHeaders = new List<string>(fixedHeaders) { "Wk1", "Wk2", "Wk3", "Wk4", "Wk5" };
                    //for (int col = 0; col < allHeaders.Count; col++)
                    //{
                    //    ws.Cells[headerRow, col + 1].Value = allHeaders[col];
                    //    ws.Cells[headerRow, col + 1].Style.Font.Bold = true;
                    //}

                    for (int col = 0; col < fixedHeaders.Count; col++)
                    {
                        ws.Cells[headerRow, col + 1].Value = fixedHeaders[col];
                        ws.Cells[headerRow, col + 1].Style.Font.Bold = true;
                    }


                    //// Freeze panes below headers
                    //ws.View.FreezePanes(headerRow + 1, 1);

                    // Repeat column labels when printing
                    ws.PrinterSettings.RepeatRows = ws.Cells[$"{headerRow}:{headerRow}"];

                    row++;

                    var groupedByArea = clientGroup.GroupBy(x => x.AreaName);
                    foreach (var areaGroup in groupedByArea)
                    {
                        ws.Cells[row, 1].Value = areaGroup.Key;
                        ws.Cells[row, 1].Style.Font.Bold = true;
                        row++;

                        foreach (var item in areaGroup)
                        {
                            ws.Cells[row, 1].Value = item.CleanFrequency;

                            var weekValues = new[] { item.Wk1, item.Wk2, item.Wk3, item.Wk4, item.Wk5 };
                            for (int i = 0; i < sundays.Count; i++)
                            {
                                ws.Cells[row, 2 + i].Value = weekValues[i];
                            }

                            ws.Cells[row, 1].Value = item.ItemCode;
                            ws.Cells[row, 2].Value = item.ClientReference;
                            ws.Cells[row, 3].Value = item.MdId;
                            ws.Cells[row, 4].Value = item.RegionName;
                            ws.Cells[row, 5].Value = item.AreaName;
                            ws.Cells[row, 6].Value = item.AssetLocation;

                            row++;
                        }
                        row++; // Blank row between areas
                    }

                    ws.Cells[1, 1, row, fixedHeaders.Count].AutoFitColumns();

                    // Conditional formatting on cleaning frequency
                    var freqRange = ws.Cells[1, 4, row, 4];

                    var red = freqRange.ConditionalFormatting.AddEqual();
                    red.Formula = "\"M\"";
                    red.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    red.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);

                    var green = freqRange.ConditionalFormatting.AddEqual();
                    green.Formula = "\"Q\"";
                    green.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    green.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);

                    await package.SaveAsAsync(new FileInfo(fullPath));

                    return fullPath;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating 'Generate Export client schedule to Excel Async Async.");

                throw new ApplicationException("Error generating 'Generate Export client schedule to Excel Async", ex);
            }
        }

        public async Task<string> GenerateExportStaffScheduleToExcelAsync(ExportStaffScheduleToExcelViewModel model)
        {
            _logger.LogInformation("Generate Export staff schedule to Excel Async.");
            try
            {

                // Adjust DateFrom to first Sunday of month, and DateTo to following Saturday last day of month if needed
                var firstSunday = DateTime.Parse(model.DateFrom.ToString());
                while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
                    firstSunday = firstSunday.AddDays(1);

                var lastSaturday = DateTime.Parse(model.DateTo.ToString());
                while (lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(1);

                model.DateFrom = firstSunday;
                model.DateTo = lastSaturday;

                var companyInfo = await _companyInfoService.GetCompanyInfoViewModelAsync();
                var cleanerSchedules = await _unitOfWork.DapperRepository.QueryAsync<ExportStaffScheduleToExcelDto>(
                "ExportStaffScheduleToExcel",
                new
                {
                    model.DateFrom,
                    model.DateTo,
                    StaffId = model.StaffId ?? 0,
                    ClientId = model.ClientId ?? 0,
                    RegionId = model.RegionId ?? 0,
                    AreaId = model.AreaId ?? 0,
                    AssetLocationId = model.AssetLocationId ?? 0,
                    AssetId = model.AssetId ?? 0
                },
                commandType: CommandType.StoredProcedure);

                var outputPath = await _settingService.GetSettingValueAsync("ExcelExportPath");
                if (string.IsNullOrWhiteSpace(outputPath))
                    throw new Exception("Excel export path is not configured.");

                var totalDays = (lastSaturday - firstSunday).Days;
                var sundays = Enumerable.Range(0, totalDays + 1)
                    .Select(d => firstSunday.AddDays(d))
                    .Where(d => d.DayOfWeek == DayOfWeek.Sunday)
                    .Select(d => DateOnly.FromDateTime(d))
                    .ToList();

                var groupedByCleaner = cleanerSchedules
                        .GroupBy(x => new { x.staffId, x.CleanerName })
                        .OrderBy(g => g.Key.CleanerName)
                        .ThenBy(g => g.Key.staffId);

                foreach (var cleanerGroup in groupedByCleaner)
                {
                    var cleanerName = cleanerGroup.Key;
                    var monthFolderName = model.DateFrom.ToString();
                    var parsedDate = DateTime.Parse(monthFolderName);
                    var folder = Path.Combine(outputPath, parsedDate.ToString("yyyy MM"));
                    Directory.CreateDirectory(folder);

                    var baseFileName = $"Schedule {model.DateFrom:yyyy_MM} {cleanerName}.xlsx";
                    var fullPath = Path.Combine(folder, baseFileName);
                    if (File.Exists(fullPath))
                    {
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
                        baseFileName = $"Schedule {model.DateFrom:yyyy_MM} {cleanerName} {timestamp}.xlsx";
                        fullPath = Path.Combine(folder, baseFileName);
                    }

                    using var package = new ExcelPackage();
                    var ws = package.Workbook.Worksheets.Add("Schedule");

                    int row = 1;

                    // Company info: Name, phone, email (left), Address (right)
                    ws.Cells[row, 1, row, 4].Merge = true;
                    ws.Cells[row, 1].Value = $"{companyInfo.Name}\nPhone: {companyInfo.Phone}\nEmail: {companyInfo.Email}";

                    ws.Cells[row, 6, row, 8].Merge = true;
                    ws.Cells[row, 6].Value = companyInfo.Address;

                    row += 4;

                    // Cleaner block left
                    ws.Cells[row, 1, row + 2, 4].Merge = true;
                    ws.Cells[row, 1].Value = cleanerGroup.First().CleanerAddress;
                    ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    // Schedule title right
                    ws.Cells[row + 1, 6].Value = $"Schedule for {model.DateFrom:MMMM yyyy}";
                    ws.Cells[row + 1, 6].Style.Font.Bold = true;

                    row += 4;

                    var headers = new List<string> { "Location", "MD ID", "Bank", "Freq" };
                    headers.AddRange(sundays.Select(d => d.ToString("d MMM")));

                    // Column headers
                    for (int col = 0; col < headers.Count; col++)
                    {
                        ws.Cells[row, col + 1].Value = headers[col];
                        ws.Cells[row, col + 1].Style.Font.Bold = true;
                    }
                    ws.PrinterSettings.RepeatRows = ws.Cells[$"{row}:{row}"];

                    row++;

                    var groupedByArea = cleanerGroup.GroupBy(x => x.AreaName);
                    foreach (var areaGroup in groupedByArea)
                    {
                        ws.Cells[row, 1].Value = areaGroup.Key;
                        ws.Cells[row, 1].Style.Font.Bold = true;
                        row++;

                        foreach (var item in areaGroup)
                        {
                            ws.Cells[row, 1].Value = item.AssetLocation;
                            ws.Cells[row, 2].Value = item.MdId;
                            ws.Cells[row, 3].Value = item.BankName;
                            ws.Cells[row, 4].Value = item.CleanFrequency;

                            var weekValues = new[] { item.Week1, item.Week2, item.Week3, item.Week4, item.Week5 };
                            for (int i = 0; i < sundays.Count; i++)
                            {
                                ws.Cells[row, 5 + i].Value = weekValues[i];
                            }
                            row++;
                        }
                        row++; // Blank row between areas
                    }

                    ws.Cells[1, 1, row, headers.Count].AutoFitColumns();

                    // Conditional formatting on cleaning frequency
                    var freqRange = ws.Cells[1, 4, row, 4];

                    var red = freqRange.ConditionalFormatting.AddEqual();
                    red.Formula = "\"M\"";
                    red.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    red.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);

                    var green = freqRange.ConditionalFormatting.AddEqual();
                    green.Formula = "\"Q\"";
                    green.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    green.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);

                    await package.SaveAsAsync(new FileInfo(fullPath));

                    return fullPath;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating 'Generate Export staff schedule to Excel Async.");

                throw new ApplicationException("Error generating 'Generate Export staff schedule to Excel Async", ex);
            }
        }
    }
}
