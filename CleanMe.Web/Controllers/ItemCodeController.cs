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
    public class ItemCodeController : Controller
    {
        private readonly IItemCodeService _ItemCodeService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ItemCodeController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public ItemCodeController(
            IItemCodeService ItemCodeService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<ItemCodeController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _ItemCodeService = ItemCodeService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? code, string? itemName, 
            string? purchasesUnitRate, string? purchasesXeroAccount, string? salesUnitRate, string? salesXeroAccount,
            string? isActive,
            string sortColumn = "Name", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Code = code;
            ViewBag.ItemName = itemName;
            ViewBag.IsActive = isActive;

            try
            {
                var ItemCodeList = await _ItemCodeService.GetItemCodeIndexAsync(
                    code, itemName,
                    purchasesUnitRate, purchasesXeroAccount, salesUnitRate, salesXeroAccount,
                    isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(ItemCodeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Item code list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Item codes.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int? itemCodeId, string? returnUrl = null)
        {
            try
            {
                ItemCodeViewModel model;

                if (itemCodeId.HasValue) // Edit Mode
                {
                    model = await _ItemCodeService.GetItemCodeViewModelWithItemCodeRatesByIdAsync(itemCodeId.Value);
                    if (model == null)
                    {
                        return NotFound();
                    }
                }
                else // Create Mode
                {
                    model = new ItemCodeViewModel();
                }

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Item code details.");
                TempData["ErrorMessage"] = "An error occurred while loading Item code detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update ItemCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ItemCodeViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    return View(model);
                }

                // Check for duplicate ItemCode (excluding current record)
                var duplicateItemCode = await _ItemCodeService.FindDuplicateItemCodeAsync(model.Code, model.ItemName, model.itemCodeId);
                if (duplicateItemCode.Any())
                {
                    //TempData["WarningMessage"] = "A ItemCode with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateItemCode.Select(s => s.itemCodeId).ToArray();
                    ModelState.AddModelError("Name", "A Item code with the same code or name already exists.");
                    return View(model);
                }

                // Add new ItemCode
                if (model.itemCodeId == 0)
                {
                    int newItemCodeId = await _ItemCodeService.AddItemCodeAsync(model, GetCurrentUserId());
                }
                else // Update Existing ItemCode
                {
                    var existingItemCode = await _ItemCodeService.GetItemCodeViewModelByIdAsync(model.itemCodeId);
                    if (existingItemCode == null)
                    {
                        TempData["ErrorMessage"] = "ItemCode record not found.";
                        return RedirectToAction("Index");
                    } 

                    Console.WriteLine("DEBUG: Updating existing Item code.");
                    await _ItemCodeService.UpdateItemCodeAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"ItemCode {model.Code} updated successfully!";
                }

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                Console.WriteLine("DEBUG: Returning RedirectToAction('Index').");
                return RedirectToAction("Index"); // This is always reached

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Item code.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /ItemCode/Delete?itemCodeId=123
        public async Task<IActionResult> Delete(int itemCodeId)
        {
            var ItemCode = await _ItemCodeService.GetItemCodeViewModelByIdAsync(itemCodeId);
            if (ItemCode == null)
            {
                TempData["ErrorMessage"] = "Item code not found.";
                return RedirectToAction("Index");
            }

            return View("ConfirmDelete", ItemCode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int itemCodeId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var ItemCode = await _ItemCodeService.GetItemCodeViewModelByIdAsync(itemCodeId);
                if (ItemCode == null)
                {
                    TempData["ErrorMessage"] = "Item code not found.";
                    return RedirectToAction("Index");
                }

                var result = await _ItemCodeService.SoftDeleteItemCodeAsync(itemCodeId, userId);

                TempData["SuccessMessage"] = result ? "Item code successfully deleted." : "Failed to delete Item code.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Item code details.");
                TempData["ErrorMessage"] = "An error occurred while loading Item code detail.";
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
