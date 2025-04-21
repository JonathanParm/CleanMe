using CleanMe.Application.Interfaces;
using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMe.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only authenticated users can access
    public class AssetTypeRateController : Controller
    {
        private readonly IAssetTypeRateService _assetTypeRateService;
        private readonly ILookupService _lookupService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AssetTypeRateController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public AssetTypeRateController(
            IAssetTypeRateService assetTypeRateService,
            ILookupService lookupService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AssetTypeRateController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _assetTypeRateService = assetTypeRateService;
            _lookupService = lookupService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
                string? rateName, string? assetTypeName, string? cleanFreqName, string? clientName,
                decimal? rate, string? isDefault, string? isActive,
                string sortColumn = "RateName", string sortOrder = "ASC",
                int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.RateName = rateName;
            ViewBag.AssetTypeName = assetTypeName;
            ViewBag.CleanFreqName = cleanFreqName;
            ViewBag.ClientName = clientName;
            ViewBag.Rate = rate;
            ViewBag.IsDefault = isDefault;
            ViewBag.IsActive = isActive;

            try
            {
                var AssetTypeRateList = await _assetTypeRateService.GetAssetTypeRateIndexAsync(
                    rateName, assetTypeName, cleanFreqName, clientName,
                    rate, isDefault, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(AssetTypeRateList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving AssetTypeRate list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading AssetTypeRates.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int assetTypeRateId = 0, string? returnUrl = null)
        {
            try
            {
                var model = assetTypeRateId > 0
                    ? await _assetTypeRateService.GetAssetTypeRateViewModelByIdAsync(assetTypeRateId)
                    : new AssetTypeRateViewModel();

                await PopulateSelectListsAsync(model, assetTypeRateId > 0);

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving AssetTypeRate details.");
                TempData["ErrorMessage"] = "An error occurred while loading AssetTypeRate detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update AssetTypeRate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AssetTypeRateViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (model.assetTypeId == null)
                {
                    ModelState.AddModelError("assetTypeId", "Asset type is required.");
                }

                if (model.cleanFrequencyId == null)
                {
                    ModelState.AddModelError("cleanFrequencyId", "Clean frequency is required.");
                }

                if (model.clientId == null)
                {
                    ModelState.AddModelError("clientId", "Client is required.");
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    ViewBag.ReturnUrl = returnUrl;
                    await PopulateSelectListsAsync(model, model.assetTypeRateId > 0);
                    return View(model);
                }

                // Check for duplicate AssetTypeRate (excluding current record)
                var duplicateAssetTypeRate = await _assetTypeRateService.FindDuplicateAssetTypeRateAsync(model.assetTypeId, model.cleanFrequencyId, model.clientId, model.assetTypeRateId);
                if (duplicateAssetTypeRate.Any())
                {
                    //TempData["WarningMessage"] = "A AssetTypeRate with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateAssetTypeRate.Select(s => s.assetTypeRateId).ToArray();
                    ModelState.AddModelError("Name", "A matching asset type rate for this client already exists.");
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Add new AssetTypeRate
                if (model.assetTypeRateId == 0)
                {
                    int newassetTypeRateId = await _assetTypeRateService.AddAssetTypeRateAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Asset Type Rate {model.Name} added successfully!";
                }
                else // Update Existing AssetTypeRate
                {
                    var existingAssetTypeRate = await _assetTypeRateService.GetAssetTypeRateViewModelOnlyByIdAsync(model.assetTypeRateId);
                    if (existingAssetTypeRate == null)
                    {
                        TempData["ErrorMessage"] = "Asset type rate record not found.";
                        if (!string.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing AssetTypeRate.");
                    await _assetTypeRateService.UpdateAssetTypeRateAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"AssetTypeRate {model.Name} updated successfully!";
                }

                Console.WriteLine("DEBUG: AssetTypeRate saved successfully");
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this AssetTypeRate.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /AssetTypeRate/Delete?assetTypeRateId=123
        public async Task<IActionResult> Delete(int assetTypeRateId, string? returnUrl = null)
        {
            var AssetTypeRate = await _assetTypeRateService.GetAssetTypeRateViewModelOnlyByIdAsync(assetTypeRateId);
            if (AssetTypeRate == null)
            {
                TempData["ErrorMessage"] = "Asset type rate not found.";
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("ConfirmDelete", AssetTypeRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int assetTypeRateId, string? returnUrl = null)
        {
            try
            {
                var result = await _assetTypeRateService.SoftDeleteAssetTypeRateAsync(assetTypeRateId, GetCurrentUserId());

                TempData["SuccessMessage"] = result ? "Asset type rate successfully deleted." : "Failed to delete asset type rate.";

                // Redirect to previous page
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving AssetTypeRate details.");
                TempData["ErrorMessage"] = "An error occurred while loading asset type rate detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task PopulateSelectListsAsync(AssetTypeRateViewModel model, bool isEditMode)
        {
            if (isEditMode)
            {
                model.Clients = await _lookupService.GetClientSelectListAsync();
                model.AssetTypes = await _lookupService.GetAssetTypeSelectListAsync();
                model.CleanFrequencies = await _lookupService.GetCleanFrequencySelectListAsync();
            }
            else
            {
                model.Clients = new[]
                {
                        new SelectListItem { Value = "", Text = "-- Select Client --" }
                    }.Concat(await _lookupService.GetClientSelectListAsync());

                model.AssetTypes = new[]
                {
                        new SelectListItem { Value = "", Text = "-- Select Type --" }
                    }.Concat(await _lookupService.GetAssetTypeSelectListAsync());

                model.CleanFrequencies = new[]
                {
                        new SelectListItem { Value = "", Text = "-- Select Frequency --" }
                    }.Concat(await _lookupService.GetCleanFrequencySelectListAsync());
            }
        }

    }
}
