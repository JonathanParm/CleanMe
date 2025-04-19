using CleanMe.Application.Interfaces;
using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanMe.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only authenticated users can access
    public class AssetLocationController : Controller
    {
        private readonly IAssetLocationService _assetLocationService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AssetLocationController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public AssetLocationController(
            IAssetLocationService assetLocationService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AssetLocationController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _assetLocationService = assetLocationService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? areaName, string? description, string? townSuburb, string? reportCode, string? isActive,
            string sortColumn = "SequenceOrder", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.AreaName = areaName;
            ViewBag.Description = description;
            ViewBag.TownSuburb = townSuburb;
            ViewBag.ReportCode = reportCode;
            ViewBag.IsActive = isActive;

            try
            {
                var assetLocationList = await _assetLocationService.GetAssetLocationIndexAsync(
                    areaName, description, townSuburb, reportCode, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(assetLocationList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset Location list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Asset Locations.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int assetLocationId = 0, int areaId = 0, string? returnUrl = null)
        {
            try
            {
                var model = assetLocationId > 0
                    ? await _assetLocationService.GetAssetLocationViewModelByIdAsync(assetLocationId)
                    : await _assetLocationService.PrepareNewAssetLocationViewModelAsync(areaId);

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset Location details.");
                TempData["ErrorMessage"] = "An error occurred while loading Asset Location detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update AssetLocation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AssetLocationViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AssetLocation AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Check for duplicate AssetLocation (excluding current record)
                var duplicateAssetLocation = await _assetLocationService.FindDuplicateAssetLocationAsync(model.Description, model.ReportCode, model.assetLocationId);
                if (duplicateAssetLocation.Any())
                {
                    //TempData["WarningMessage"] = "A AssetLocation with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateAssetLocation.Select(s => s.assetLocationId).ToArray();
                    ModelState.AddModelError("Name", "An Asset Location with the same description or report code already exists.");
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Add new AssetLocation
                if (model.assetLocationId == 0)
                {
                    int newassetLocationId = await _assetLocationService.AddAssetLocationAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Asset Location {model.Description} added successfully!";
                }
                else // Update Existing AssetLocation
                {
                    var existingAssetLocation = await _assetLocationService.GetAssetLocationViewModelByIdAsync(model.assetLocationId);
                    if (existingAssetLocation == null)
                    {
                        TempData["ErrorMessage"] = "Asset Location record not found.";
                        if (!string.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Asset Location member.");
                    await _assetLocationService.UpdateAssetLocationAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Asset Location {model.Description} updated successfully!";
                }

                Console.WriteLine("DEBUG: Asset Location saved successfully");
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Asset Location.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /AssetLocation/Delete?assetLocationId=123
        public async Task<IActionResult> Delete(int assetLocationId, string? returnUrl = null)
        {
            var AssetLocation = await _assetLocationService.GetAssetLocationViewModelByIdAsync(assetLocationId);
            if (AssetLocation == null)
            {
                TempData["ErrorMessage"] = "Asset Location not found.";
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("ConfirmDelete", AssetLocation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int assetLocationId, string? returnUrl = null)
        {
            try
            {
                var result = await _assetLocationService.SoftDeleteAssetLocationAsync(assetLocationId, GetCurrentUserId());

                TempData["SuccessMessage"] = result ? "Asset Location successfully deleted." : "Failed to delete Asset Location.";

                // Redirect to previous page
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset Location details.");
                TempData["ErrorMessage"] = "An error occurred while loading Asset Location detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
