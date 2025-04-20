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
    public class CleanFrequencyController : Controller
    {
        private readonly ICleanFrequencyService _cleanFrequencyService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CleanFrequencyController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public CleanFrequencyController(
            ICleanFrequencyService cleanFrequencyService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<CleanFrequencyController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _cleanFrequencyService = cleanFrequencyService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? name, string? description, string? code, string? isActive,
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
                var CleanFrequencyList = await _cleanFrequencyService.GetCleanFrequencyIndexAsync(
                    name, description, code, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(CleanFrequencyList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Clean Frequency list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Clean Frequencies.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int? cleanFrequencyId)
        {
            try
            {
                CleanFrequencyViewModel model;

                if (cleanFrequencyId.HasValue) // Edit Mode
                {
                    model = await _cleanFrequencyService.GetCleanFrequencyViewModelByIdAsync(cleanFrequencyId.Value);
                    if (model == null)
                    {
                        return NotFound();
                    }
                }
                else // Create Mode
                {
                    model = new CleanFrequencyViewModel();
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Clean Frequency details.");
                TempData["ErrorMessage"] = "An error occurred while loading CleanFrequency detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update CleanFrequency
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(CleanFrequencyViewModel model)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    return View(model);
                }

                // Check for duplicate CleanFrequency (excluding current record)
                var duplicateCleanFrequency = await _cleanFrequencyService.FindDuplicateCleanFrequencyAsync(model.Name, model.Code, model.cleanFrequencyId);
                if (duplicateCleanFrequency.Any())
                {
                    //TempData["WarningMessage"] = "A CleanFrequency with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateCleanFrequency.Select(s => s.cleanFrequencyId).ToArray();
                    ModelState.AddModelError("Name", "A Clean Frequency with the same name or code already exists.");
                    return View(model);
                }

                // Add new CleanFrequency
                if (model.cleanFrequencyId == 0)
                {
                    int newcleanFrequencyId = await _cleanFrequencyService.AddCleanFrequencyAsync(model, GetCurrentUserId());
                }
                else // Update Existing CleanFrequency
                {
                    var existingCleanFrequency = await _cleanFrequencyService.GetCleanFrequencyViewModelByIdAsync(model.cleanFrequencyId);
                    if (existingCleanFrequency == null)
                    {
                        TempData["ErrorMessage"] = "Clean Frequency record not found.";
                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Clean Frequency member.");
                    await _cleanFrequencyService.UpdateCleanFrequencyAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Clean Frequency {model.Name} updated successfully!";
                }

                Console.WriteLine("DEBUG: Returning RedirectToAction('Index').");
                return RedirectToAction("Index"); // This is always reached

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing cleaning frequency.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /CleanFrequency/Delete?cleanFrequencyId=123
        public async Task<IActionResult> Delete(int cleanFrequencyId)
        {
            var CleanFrequency = await _cleanFrequencyService.GetCleanFrequencyViewModelByIdAsync(cleanFrequencyId);
            if (CleanFrequency == null)
            {
                TempData["ErrorMessage"] = "Clean Frequency not found.";
                return RedirectToAction("Index");
            }

            return View("ConfirmDelete", CleanFrequency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int cleanFrequencyId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var CleanFrequency = await _cleanFrequencyService.GetCleanFrequencyViewModelByIdAsync(cleanFrequencyId);
                if (CleanFrequency == null)
                {
                    TempData["ErrorMessage"] = "Clean Frequency not found.";
                    return RedirectToAction("Index");
                }

                var result = await _cleanFrequencyService.SoftDeleteCleanFrequencyAsync(cleanFrequencyId, userId);

                TempData["SuccessMessage"] = result ? "Clean Frequency successfully deleted." : "Failed to delete Clean Frequency.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Clean Frequency details.");
                TempData["ErrorMessage"] = "An error occurred while loading Clean Frequency detail.";
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
