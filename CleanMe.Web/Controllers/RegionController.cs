using CleanMe.Application.Interfaces;
using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
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
            string? name, string? code, string? isActive,
                string sortColumn = "SequenceOrder", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Name = name;
            ViewBag.Code = code;
            ViewBag.IsActive = isActive;

            try
            {
                var regionList = await _regionService.GetRegionIndexAsync(
                    name, code, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

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
        public async Task<IActionResult> AddEdit(int? regionId, string? returnUrl = null)
        {
            try
            {
                RegionViewModel model;

                if (regionId.HasValue) // Edit Mode
                {
                    model = await _regionService.GetRegionWithAreasByIdAsync(regionId.Value);
                    if (model == null)
                    {
                        return NotFound();
                    }
                }
                else // Create Mode
                {
                    model = new RegionViewModel();
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
        public async Task<IActionResult> AddEdit(RegionViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    return View(model);
                }

                // Check for duplicate region (excluding current record)
                var duplicateRegion = await _regionService.FindDuplicateRegionAsync(model.Name, model.ReportCode, model.regionId);
                if (duplicateRegion.Any())
                {
                    //TempData["WarningMessage"] = "A region with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateRegion.Select(s => s.regionId).ToArray();
                    ModelState.AddModelError("Name", "A region with the same name or code already exists.");
                    return View(model);
                }

                // Add new Region
                if (model.regionId == 0)
                {
                    int newRegionId = await _regionService.AddRegionAsync(model, GetCurrentUserId());
                }
                else // Update Existing Region
                {
                    var existingRegion = await _regionService.GetRegionByIdAsync(model.regionId);
                    if (existingRegion == null)
                    {
                        TempData["ErrorMessage"] = "Region record not found.";
                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Region member.");
                    await _regionService.UpdateRegionAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Region {model.Name} updated successfully!";
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
            var region = await _regionService.GetRegionByIdAsync(regionId);
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
                var region = await _regionService.GetRegionByIdAsync(regionId);
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
    }
}
