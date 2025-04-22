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
    public class StockCodeController : Controller
    {
        private readonly IStockCodeService _stockCodeService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StockCodeController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public StockCodeController(
            IStockCodeService stockCodeService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<StockCodeController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _stockCodeService = stockCodeService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? name, string? description, string? isActive,
                string sortColumn = "Name", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Name = name;
            ViewBag.Description = description;
            ViewBag.IsActive = isActive;

            try
            {
                var StockCodeList = await _stockCodeService.GetStockCodeIndexAsync(
                    name, description, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(StockCodeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Stock Code list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Stock Codes.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int? stockCodeId, string? returnUrl = null)
        {
            try
            {
                StockCodeViewModel model;

                if (stockCodeId.HasValue) // Edit Mode
                {
                    model = await _stockCodeService.GetStockCodeViewModelWithAssetTypesByIdAsync(stockCodeId.Value);
                    if (model == null)
                    {
                        return NotFound();
                    }
                }
                else // Create Mode
                {
                    model = new StockCodeViewModel();
                }

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Stock Code details.");
                TempData["ErrorMessage"] = "An error occurred while loading Stock Code detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update StockCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(StockCodeViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    return View(model);
                }

                // Check for duplicate StockCode (excluding current record)
                var duplicateStockCode = await _stockCodeService.FindDuplicateStockCodeAsync(model.Name, model.stockCodeId);
                if (duplicateStockCode.Any())
                {
                    //TempData["WarningMessage"] = "A StockCode with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateStockCode.Select(s => s.stockCodeId).ToArray();
                    ModelState.AddModelError("Name", "A Stock Code with the same name or code already exists.");
                    return View(model);
                }

                // Add new StockCode
                if (model.stockCodeId == 0)
                {
                    int newstockCodeId = await _stockCodeService.AddStockCodeAsync(model, GetCurrentUserId());
                }
                else // Update Existing StockCode
                {
                    var existingStockCode = await _stockCodeService.GetStockCodeViewModelByIdAsync(model.stockCodeId);
                    if (existingStockCode == null)
                    {
                        TempData["ErrorMessage"] = "StockCode record not found.";
                        return RedirectToAction("Index");
                    } 

                    Console.WriteLine("DEBUG: Updating existing Stock Code member.");
                    await _stockCodeService.UpdateStockCodeAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"StockCode {model.Name} updated successfully!";
                }

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                Console.WriteLine("DEBUG: Returning RedirectToAction('Index').");
                return RedirectToAction("Index"); // This is always reached

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Stock Code.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /StockCode/Delete?stockCodeId=123
        public async Task<IActionResult> Delete(int stockCodeId)
        {
            var StockCode = await _stockCodeService.GetStockCodeViewModelByIdAsync(stockCodeId);
            if (StockCode == null)
            {
                TempData["ErrorMessage"] = "Stock Code not found.";
                return RedirectToAction("Index");
            }

            return View("ConfirmDelete", StockCode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int stockCodeId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var StockCode = await _stockCodeService.GetStockCodeViewModelByIdAsync(stockCodeId);
                if (StockCode == null)
                {
                    TempData["ErrorMessage"] = "Stock Code not found.";
                    return RedirectToAction("Index");
                }

                var result = await _stockCodeService.SoftDeleteStockCodeAsync(stockCodeId, userId);

                TempData["SuccessMessage"] = result ? "Stock Code successfully deleted." : "Failed to delete Stock Code.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Stock Code details.");
                TempData["ErrorMessage"] = "An error occurred while loading Stock Code detail.";
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
