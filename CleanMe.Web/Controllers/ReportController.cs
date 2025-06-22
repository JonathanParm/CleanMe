using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanMe.Application.Interfaces;
using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CleanMe.Application.Filters;
using System.Globalization;

namespace CleanMe.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILookupService _lookupService;

        public ReportController(IReportService reportService, ILookupService lookupService)
        {
            _reportService = reportService;
            _lookupService = lookupService;
        }

        public async Task<IActionResult> ScheduleExportToExcel()
        {
            var model = new ScheduleExportToExcelViewModel();

            await PopulateSelectListsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleExportToExcel(ScheduleExportToExcelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        Console.WriteLine($"{key} has errors:");
                        foreach (var error in state.Errors)
                            Console.WriteLine($"  {error.ErrorMessage}");
                    }
                }
                // If the model state is invalid, repopulate the select lists and return the view with the model
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            var filePath = await _reportService.GenerateScheduleExportToExcelAsync(model);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        private async Task PopulateSelectListsAsync(ScheduleExportToExcelViewModel model)
        {
            // the default date range is the next month
            var now = DateTime.Now;
            var isDecember = now.Month == 12;

            var selectedMonth = isDecember ? 1 : now.Month + 1;
            var selectedYear = isDecember ? now.Year + 1 : now.Year;

            model.DateRangeType = "Month";
            model.Year = selectedYear;
            model.Month = selectedMonth;
            model.YearList = new List<SelectListItem>
            {
                new SelectListItem { Text = (now.Year - 1).ToString(), Value = (now.Year - 1).ToString() },
                new SelectListItem { Text = now.Year.ToString(), Value = now.Year.ToString() },
                new SelectListItem { Text = (now.Year + 1).ToString(), Value = (now.Year + 1).ToString() },
            };
            model.MonthList = Enumerable.Range(1, 12)
                .Select(i => new SelectListItem
                {
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    Value = i.ToString()
                });

            model.Cleaners = new[]
            {
                new SelectListItem { Value = "", Text = "All staff" }
            }.Concat(await _lookupService.GetStaffSelectListAsync(new StaffLookupFilter()));

            model.Clients = new[]
            {
                new SelectListItem { Value = "", Text = "All clients" }
            }.Concat(await _lookupService.GetClientSelectListAsync());

            model.Regions = new[]
            {
                new SelectListItem { Value = "", Text = "All regions" }
            }.Concat(await _lookupService.GetRegionSelectListAsync(new RegionLookupFilter()));

            model.Areas = new[]
            {
                new SelectListItem { Value = "", Text = "All areas" }
            }.Concat(await _lookupService.GetAreaSelectListAsync(new AreaLookupFilter()));

            model.AssetLocations = new[]
            {
                new SelectListItem { Value = "", Text = "All locations" }
            }.Concat(await _lookupService.GetAssetLocationSelectListAsync(new AssetLocationLookupFilter()));

            model.Assets = new[]
            {
                new SelectListItem { Value = "", Text = "All assets" }
            }.Concat(await _lookupService.GetAssetSelectListAsync(new AssetLookupFilter()));
        }

        // AJAX lookups

        [HttpGet]
        public async Task<IActionResult> GetRegionIdSelectList(int clientId = 0, int cleanerId = 0)
        {
            var filter = new RegionLookupFilter { ClientId = clientId, StaffId = cleanerId };
            var items = await _lookupService.GetRegionSelectListAsync(filter);
            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> GetAreaIdSelectList(int clientId = 0, int cleanerId = 0, int regionId = 0)
        {
            var filter = new AreaLookupFilter { ClientId = clientId, StaffId = cleanerId, RegionId = regionId };
            var items = await _lookupService.GetAreaSelectListAsync(filter);
            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetLocationIdSelectList(int clientId = 0, int cleanerId = 0, int regionId = 0, int areaId = 0)
        {
            var filter = new AssetLocationLookupFilter
            {
                StaffId = cleanerId,
                ClientId = clientId,
                RegionId = regionId,
                AreaId = areaId
            };
            var items = await _lookupService.GetAssetLocationSelectListAsync(filter);
            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetIdSelectList(int clientId = 0, int cleanerId = 0, int regionId = 0, int areaId = 0, int assetLocationId = 0)
        {
            var filter = new AssetLookupFilter
            {
                StaffId = cleanerId,
                ClientId = clientId,
                RegionId = regionId,
                AreaId = areaId,
                AssetLocationId = assetLocationId
            };
            var items = await _lookupService.GetAssetSelectListAsync(filter);
            return Json(items);
        }

    }
}