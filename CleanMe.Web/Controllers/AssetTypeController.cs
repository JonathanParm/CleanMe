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
    public class AssetTypeController : Controller
    {
        private readonly IAssetTypeService _assetTypeService;
        private readonly ILookupService _lookupService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AssetTypeController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public AssetTypeController(
            IAssetTypeService assetTypeService,
            ILookupService lookupService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AssetTypeController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _assetTypeService = assetTypeService;
            _lookupService = lookupService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? name, string? isActive,
                string sortColumn = "Name", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Name = name;
            ViewBag.IsActive = isActive;

            try
            {
                var AssetTypeList = await _assetTypeService.GetAssetTypeIndexAsync(
                    name, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(AssetTypeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset Type list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Asset Types.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int? assetTypeId, int stockCodeId = 0, string? returnUrl = null)
        {
            try
            {

                var model = assetTypeId.Value > 0
                    ? await _assetTypeService.GetAssetTypeViewModelByIdAsync(assetTypeId.Value)
                    : await _assetTypeService.PrepareNewAssetTypeViewModelAsync(stockCodeId);


                await PopulateSelectListsAsync(model, assetTypeId > 0);

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset Type details.");
                TempData["ErrorMessage"] = "An error occurred while loading Asset Type detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update AssetType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AssetTypeViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (model.stockCodeId == null)
                {
                    ModelState.AddModelError("stockCodeId", "Stock code is required.");
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    await PopulateSelectListsAsync(model, model.assetTypeId > 0);
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Check for duplicate AssetType (excluding current record)
                var duplicateAssetType = await _assetTypeService.FindDuplicateAssetTypeAsync(model.Name, model.assetTypeId);
                if (duplicateAssetType.Any())
                {
                    //TempData["WarningMessage"] = "A Asset Type with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateAssetType.Select(s => s.assetTypeId).ToArray();
                    ModelState.AddModelError("Name", "A Asset Type with the same name or code already exists.");
                    await PopulateSelectListsAsync(model, model.assetTypeId > 0);
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Add new AssetType
                if (model.assetTypeId == 0)
                {
                    int newAssetTypeId = await _assetTypeService.AddAssetTypeAsync(model, GetCurrentUserId());
                }
                else // Update Existing Asset Type
                {
                    var existingAssetType = await _assetTypeService.GetAssetTypeViewModelByIdAsync(model.assetTypeId);
                    if (existingAssetType == null)
                    {
                        TempData["ErrorMessage"] = "Asset Type record not found.";
                        if (!string.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Asset Type member.");
                    await _assetTypeService.UpdateAssetTypeAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Asset Type {model.Name} updated successfully!";
                }

                Console.WriteLine("DEBUG: Returning RedirectToAction('Index').");
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index"); // This is always reached

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Asset Type.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /AssetType/Delete?AssetTypeId=123
        public async Task<IActionResult> Delete(int assetTypeId)
        {
            var AssetType = await _assetTypeService.GetAssetTypeViewModelByIdAsync(assetTypeId);
            if (AssetType == null)
            {
                TempData["ErrorMessage"] = "Asset Type not found.";
                return RedirectToAction("Index");
            }

            return View("ConfirmDelete", AssetType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int assetTypeId, string? returnUrl = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var AssetType = await _assetTypeService.GetAssetTypeViewModelByIdAsync(assetTypeId);
                if (AssetType == null)
                {
                    TempData["ErrorMessage"] = "Asset Type not found.";
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index");
                }

                var result = await _assetTypeService.SoftDeleteAssetTypeAsync(assetTypeId, userId);

                TempData["SuccessMessage"] = result ? "Asset Type successfully deleted." : "Failed to delete Asset Type.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset Type details.");
                TempData["ErrorMessage"] = "An error occurred while loading Asset Type detail.";
                return RedirectToAction("HandleError", "Error");
            }

            ViewBag.ReturnUrl = returnUrl; 
            return RedirectToAction("Index");
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task PopulateSelectListsAsync(AssetTypeViewModel model, bool isEditMode)
        {
            if (isEditMode)
            {
                model.StockCodes = await _lookupService.GetStockCodeSelectListAsync();
            }
            else
            {
                model.StockCodes = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select Stock Code --" }
                }.Concat(await _lookupService.GetStockCodeSelectListAsync());
            }
        }
    }
}
