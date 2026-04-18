using CleanMe.Application.Helpers.Paging;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanMe.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only authenticated users can access
    public class RegionController : Controller
    {
        private readonly IRegionService _regionService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegionController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public RegionController(
            IRegionService regionService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<RegionController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _regionService = regionService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? regionName, string? reportCode, string? isActive,
                string sortColumn = "RegionName", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 5)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Name = regionName;
            ViewBag.Code = reportCode;
            ViewBag.IsActive = isActive;

            try
            {
                //var regionList = await _regionService.GetRegionIndexAsync(
                //    name, code, isActive,
                //    sortColumn, sortOrder, pageNumber, pageSize);

                var regionList = await _regionService.GetPagedIndexAsync(
                    pageNumber,
                    pageSize,
                    sortColumn,
                    sortOrder,
                    regionName,
                    reportCode,
                    isActive);

                return View(regionList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving region list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading regions.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int? regionId, int pageNumber = 1, int pageSize = 5, string? returnUrl = null)
        {
            try
            {
                RegionWithAreasViewModel model;

                if (regionId.HasValue) // Edit Mode
                {
                    model = await _regionService.GetRegionWithAreasViewModelByIdAsync(regionId.Value, pageNumber, pageSize);
                    if (model == null)
                    {
                        return NotFound();
                    }
                }
                else // Create Mode
                {
                    model = new RegionWithAreasViewModel
                    {
                        RegionViewModel = new RegionViewModel
                        {
                            PageNumber = pageNumber,
                            PageSize = pageSize,
                            TotalCount = 0
                        },
                        Areas = Array.Empty<Area>()
                    };
                }

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving region details.");
                TempData["ErrorMessage"] = "An error occurred while loading region detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update Region
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(RegionWithAreasViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    TempData.Remove("SuccessMessage");
                    ViewBag.ReturnUrl = returnUrl;
                    var vm = await BuildRegionWithAreasViewModelForReturn(model.RegionViewModel);
                    return View(vm);
                }

                // Check for duplicate region (excluding current record)
                var duplicateRegion = await _regionService.FindDuplicateRegionAsync(model.RegionViewModel.RegionName, model.RegionViewModel.ReportCode, model.RegionViewModel.regionId);
                if (duplicateRegion.Any())
                {
                    ModelState.AddModelError("Name", "A region with the same name or code already exists.");
                    ViewBag.ReturnUrl = returnUrl;
                    var vm = await BuildRegionWithAreasViewModelForReturn(model.RegionViewModel);
                    return View(vm);
                }

                // Add new Region
                if (model.RegionViewModel.regionId == 0)
                {
                    int newRegionId = await _regionService.AddRegionAsync(model.RegionViewModel, GetCurrentUserId());
                }
                else // Update Existing Region
                {
                    var existingRegion = await _regionService.GetRegionViewModelByIdAsync(model.RegionViewModel.regionId);
                    if (existingRegion == null)
                    {
                        TempData["ErrorMessage"] = "Region record not found.";
                        TempData.Remove("SuccessMessage");
                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Region member.");
                    await _regionService.UpdateRegionAsync(model.RegionViewModel, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Region {model.RegionViewModel.RegionName} updated successfully!";
                    TempData.Remove("ErrorMessage");
                }

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                Console.WriteLine("DEBUG: Returning RedirectToAction('Index').");
                return RedirectToAction("Index"); // This is always reached

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this region.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /Region/Delete?regionId=123
        public async Task<IActionResult> Delete(int regionId)
        {
            var region = await _regionService.GetRegionViewModelByIdAsync(regionId);
            if (region == null)
            {
                TempData["ErrorMessage"] = "Region not found.";
                return RedirectToAction("Index");
            }

            return View("ConfirmDelete", region);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int regionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var region = await _regionService.GetRegionViewModelByIdAsync(regionId);
                if (region == null)
                {
                    TempData["ErrorMessage"] = "Region not found.";
                    return RedirectToAction("Index");
                }

                var result = await _regionService.SoftDeleteRegionAsync(regionId, userId);

                TempData["SuccessMessage"] = result ? "Region successfully deleted." : "Failed to delete region.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving region details.");
                TempData["ErrorMessage"] = "An error occurred while loading region detail.";
                return RedirectToAction("HandleError", "Error");
            }

            return RedirectToAction("Index");
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        // Helper: Build a RegionWithAreasViewModel to return to the view when a RegionViewModel is posted
        private async Task<RegionWithAreasViewModel> BuildRegionWithAreasViewModelForReturn(RegionViewModel posted)
        {
            var vm = posted.regionId > 0
                ? await _regionService.GetRegionWithAreasViewModelByIdAsync(posted.regionId, posted.PageNumber, posted.PageSize)
                  ?? new RegionWithAreasViewModel()
                : new RegionWithAreasViewModel();

            vm.RegionViewModel = new RegionViewModel
            {
                regionId = posted.regionId,
                RegionName = posted.RegionName,
                ReportCode = posted.ReportCode,
                SortOrder = posted.SortOrder,
                IsActive = posted.IsActive,
                PageNumber = posted.PageNumber,
                PageSize = posted.PageSize,
                TotalCount = posted.TotalCount
            };

            return vm;
        }
    }
}
