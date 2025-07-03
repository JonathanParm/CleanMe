using CleanMe.Application.Filters;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMe.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only authenticated users can access
    public class AssetController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly ILookupService _lookupService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AssetController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public AssetController(
            IAssetService assetService,
            ILookupService lookupService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AssetController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _assetService = assetService;
            _lookupService = lookupService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? assetName, string? regionName, string? mdReference, string? clientName, string? clientReference,
            string? assetLocation, string? assetType,
            string sortColumn = "AssetName", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.AssetName = assetName;
            ViewBag.RegionName = regionName;
            ViewBag.MdReference = mdReference;
            ViewBag.ClientName = clientName;
            ViewBag.ClientReference = clientReference;
            ViewBag.AssetLocation = assetLocation;
            ViewBag.AssetType = assetType;

            try
            {
                var AssetList = await _assetService.GetAssetIndexAsync(
                    assetName, regionName, mdReference, clientName, clientReference,
                    assetLocation, assetType,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(AssetList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Assets.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int assetId = 0, string? returnUrl = null)
        {
            try
            {
                var model = assetId > 0
                    ? await _assetService.GetAssetViewModelByIdAsync(assetId)
                    : new AssetViewModel();

                await PopulateSelectListsAsync(model, assetId > 0);

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset details.");
                TempData["ErrorMessage"] = "An error occurred while loading Asset detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update Asset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AssetViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (model.clientId == null)
                {
                    ModelState.AddModelError("clientId", "Client is required.");
                }

                if (model.assetLocationId == null)
                {
                    ModelState.AddModelError("assetLocationId", "Asset location is required.");
                }

                if (model.itemCodeId == null)
                {
                    ModelState.AddModelError("itemCodeId", "Item code is required.");
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    ViewBag.ReturnUrl = returnUrl;
                    await PopulateSelectListsAsync(model, model.assetId > 0);
                    return View(model);
                }

                // Check for duplicate Asset (excluding current record)
                var duplicateAsset = await _assetService.FindDuplicateAssetAsync(model.Name, model.assetId);
                if (duplicateAsset.Any())
                {
                    //TempData["WarningMessage"] = "A Asset with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateAsset.Select(s => s.assetId).ToArray();
                    ModelState.AddModelError("Name", "An Asset with the same name already exists.");
                    ViewBag.ReturnUrl = returnUrl;
                    await PopulateSelectListsAsync(model, model.assetId > 0);
                    return View(model);
                }

                // Add new Asset
                if (model.assetId == 0)
                {
                    int newassetId = await _assetService.AddAssetAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Asset {model.Name} added successfully!";
                }
                else // Update Existing Asset
                {
                    var existingAsset = await _assetService.GetAssetViewModelByIdAsync(model.assetId);
                    if (existingAsset == null)
                    {
                        TempData["ErrorMessage"] = "Asset record not found.";
                        if (!string.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Asset.");
                    await _assetService.UpdateAssetAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Asset {model.Name} updated successfully!";
                }

                Console.WriteLine("DEBUG: Asset saved successfully");
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Asset.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /Asset/Delete?assetId=123
        public async Task<IActionResult> Delete(int assetId, string? returnUrl = null)
        {
            var Asset = await _assetService.GetAssetViewModelByIdAsync(assetId);
            if (Asset == null)
            {
                TempData["ErrorMessage"] = "Asset not found.";
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("ConfirmDelete", Asset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int assetId, string? returnUrl = null)
        {
            try
            {
                var result = await _assetService.SoftDeleteAssetAsync(assetId, GetCurrentUserId());

                TempData["SuccessMessage"] = result ? "Asset successfully deleted." : "Failed to delete Asset.";

                // Redirect to previous page
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset details.");
                TempData["ErrorMessage"] = "An error occurred while loading Asset detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task PopulateSelectListsAsync(AssetViewModel model, bool isEditMode)
        {
            if (isEditMode)
            {
                model.Clients = await _lookupService.GetClientSelectListAsync();
                model.AssetLocations = await _lookupService.GetAssetLocationSelectListAsync(new AssetLocationLookupFilter());
                model.ItemCodes = await _lookupService.GetItemCodeSelectListAsync(new ItemCodeLookupFilter());
            }
            else
            {
                model.Clients = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select client --" }
                }.Concat(await _lookupService.GetClientSelectListAsync());

                model.AssetLocations = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select location --" }
                }.Concat(await _lookupService.GetAssetLocationSelectListAsync(new AssetLocationLookupFilter()));

                model.ItemCodes = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select item code --" }
                }.Concat(await _lookupService.GetItemCodeSelectListAsync(new ItemCodeLookupFilter()));
            }
        }

    }
}
