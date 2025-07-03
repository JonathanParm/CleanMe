using CleanMe.Application.Filters;
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
    public class ItemCodeRateController : Controller
    {
        private readonly IItemCodeRateService _itemCodeRateService;
        private readonly ILookupService _lookupService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ItemCodeRateController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public ItemCodeRateController(
            IItemCodeRateService itemCodeRateService,
            ILookupService lookupService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<ItemCodeRateController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _itemCodeRateService = itemCodeRateService;
            _lookupService = lookupService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
                string? rateName, string? ItemCodeName, string? cleanFreqName, string? clientName,
                decimal? rate, string? isDefault, string? isActive,
                string sortColumn = "RateName", string sortOrder = "ASC",
                int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.RateName = rateName;
            ViewBag.ItemCodeName = ItemCodeName;
            ViewBag.CleanFreqName = cleanFreqName;
            ViewBag.ClientName = clientName;
            ViewBag.Rate = rate;
            ViewBag.IsDefault = isDefault;
            ViewBag.IsActive = isActive;

            try
            {
                var ItemCodeRateList = await _itemCodeRateService.GetItemCodeRateIndexAsync(
                    rateName, ItemCodeName, cleanFreqName, clientName,
                    rate, isDefault, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(ItemCodeRateList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ItemCodeRate list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading ItemCodeRates.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int itemCodeRateId = 0, string? returnUrl = null)
        {
            try
            {
                var model = itemCodeRateId > 0
                    ? await _itemCodeRateService.GetItemCodeRateViewModelByIdAsync(itemCodeRateId)
                    : new ItemCodeRateViewModel();

                await PopulateSelectListsAsync(model, itemCodeRateId > 0);

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Item Code Rate details.");
                TempData["ErrorMessage"] = "An error occurred while loading Item Code Rate detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update ItemCodeRate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ItemCodeRateViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                //if (model.clientId == null)
                //{
                //    ModelState.AddModelError("clientId", "Client is required.");
                //}

                //if (model.itemCodeId == null)
                //{
                //    ModelState.AddModelError("itemCodeId", "Asset type is required.");
                //}

                //if (model.cleanFrequencyId == null)
                //{
                //    ModelState.AddModelError("cleanFrequencyId", "Clean frequency is required.");
                //}

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    ViewBag.ReturnUrl = returnUrl;
                    await PopulateSelectListsAsync(model, model.itemCodeRateId > 0);
                    return View(model);
                }

                // Check for duplicate ItemCodeRate (excluding current record)
                var duplicateItemCodeRate = await _itemCodeRateService.FindDuplicateItemCodeRateAsync(model.itemCodeId.Value, model.cleanFrequencyId.Value, model.clientId.Value, model.itemCodeRateId);
                if (duplicateItemCodeRate.Any())
                {
                    //TempData["WarningMessage"] = "An ItemCodeRate with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateItemCodeRate.Select(s => s.itemCodeRateId).ToArray();
                    ModelState.AddModelError("Name", "A matching item code rate for this client already exists.");
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Add new ItemCodeRate
                if (model.itemCodeRateId == 0)
                {
                    int newItemCodeRateId = await _itemCodeRateService.AddItemCodeRateAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Item Code Rate {model.Name} added successfully!";
                }
                else // Update Existing ItemCodeRate
                {
                    var existingItemCodeRate = await _itemCodeRateService.GetItemCodeRateViewModelOnlyByIdAsync(model.itemCodeRateId);
                    if (existingItemCodeRate == null)
                    {
                        TempData["ErrorMessage"] = "Item code rate record not found.";
                        if (!string.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Item Code Rate.");
                    await _itemCodeRateService.UpdateItemCodeRateAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Item Code Rate {model.Name} updated successfully!";
                }

                Console.WriteLine("DEBUG: Item Code Rate saved successfully");
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Item Code Rate.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /ItemCodeRate/Delete?itemCodeRateId=123
        public async Task<IActionResult> Delete(int itemCodeRateId, string? returnUrl = null)
        {
            var ItemCodeRate = await _itemCodeRateService.GetItemCodeRateViewModelOnlyByIdAsync(itemCodeRateId);
            if (ItemCodeRate == null)
            {
                TempData["ErrorMessage"] = "Item code rate not found.";
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("ConfirmDelete", ItemCodeRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int itemCodeRateId, string? returnUrl = null)
        {
            try
            {
                var result = await _itemCodeRateService.SoftDeleteItemCodeRateAsync(itemCodeRateId, GetCurrentUserId());

                TempData["SuccessMessage"] = result ? "Item code rate successfully deleted." : "Failed to delete Item code rate.";

                // Redirect to previous page
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Item Code Rate details.");
                TempData["ErrorMessage"] = "An error occurred while loading Item code rate detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task PopulateSelectListsAsync(ItemCodeRateViewModel model, bool isEditMode)
        {
            if (isEditMode)
            {
                model.Clients = await _lookupService.GetClientSelectListAsync();
                model.ItemCodes = await _lookupService.GetItemCodeSelectListAsync(new ItemCodeLookupFilter());
                model.CleanFrequencies = await _lookupService.GetCleanFrequencySelectListAsync();
            }
            else
            {
                model.Clients = new[]
                {
                        new SelectListItem { Value = "", Text = "-- Select Client --" }
                    }.Concat(await _lookupService.GetClientSelectListAsync());

                model.ItemCodes = new[]
                {
                        new SelectListItem { Value = "", Text = "-- Select Item code --" }
                    }.Concat(await _lookupService.GetItemCodeSelectListAsync(new ItemCodeLookupFilter()));

                model.CleanFrequencies = new[]
                {
                        new SelectListItem { Value = "", Text = "-- Select Frequency --" }
                    }.Concat(await _lookupService.GetCleanFrequencySelectListAsync());
            }
        }

    }
}
