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
    public class AmendmentTypeController : Controller
    {
        private readonly IAmendmentTypeService _amendmentTypeService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AmendmentTypeController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public AmendmentTypeController(
            IAmendmentTypeService amendmentTypeService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AmendmentTypeController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _amendmentTypeService = amendmentTypeService;
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
                var AmendmentTypeList = await _amendmentTypeService.GetAmendmentTypeIndexAsync(
                    name, description, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(AmendmentTypeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving AmendmentType list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading AmendmentTypes.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int? amendmentTypeId, string? returnUrl = null)
        {
            try
            {
                AmendmentTypeViewModel model;

                if (amendmentTypeId.HasValue) // Edit Mode
                {
                    model = await _amendmentTypeService.GetAmendmentTypeViewModelByIdAsync(amendmentTypeId.Value);
                    if (model == null)
                    {
                        return NotFound();
                    }
                }
                else // Create Mode
                {
                    model = new AmendmentTypeViewModel();
                }

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Amendment Type details.");
                TempData["ErrorMessage"] = "An error occurred while loading Amendment Type detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update AmendmentType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AmendmentTypeViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    return View(model);
                }

                // Check for duplicate AmendmentType (excluding current record)
                var duplicateAmendmentType = await _amendmentTypeService.FindDuplicateAmendmentTypeAsync(model.Name, model.amendmentTypeId);
                if (duplicateAmendmentType.Any())
                {
                    //TempData["WarningMessage"] = "An Amendment Type with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateAmendmentType.Select(s => s.amendmentTypeId).ToArray();
                    ModelState.AddModelError("Name", "An Amendment Type with the same name or code already exists.");
                    return View(model);
                }

                // Add new AmendmentType
                if (model.amendmentTypeId == 0)
                {
                    int newamendmentTypeId = await _amendmentTypeService.AddAmendmentTypeAsync(model, GetCurrentUserId());
                }
                else // Update Existing AmendmentType
                {
                    var existingAmendmentType = await _amendmentTypeService.GetAmendmentTypeViewModelByIdAsync(model.amendmentTypeId);
                    if (existingAmendmentType == null)
                    {
                        TempData["ErrorMessage"] = "Amendment Type record not found.";
                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Amendment Type member.");
                    await _amendmentTypeService.UpdateAmendmentTypeAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Amendment Type {model.Name} updated successfully!";
                }

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                Console.WriteLine("DEBUG: Returning RedirectToAction('Index').");
                return RedirectToAction("Index"); // This is always reached

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Amendment Type.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /AmendmentType/Delete?amendmentTypeId=123
        public async Task<IActionResult> Delete(int amendmentTypeId)
        {
            var AmendmentType = await _amendmentTypeService.GetAmendmentTypeViewModelByIdAsync(amendmentTypeId);
            if (AmendmentType == null)
            {
                TempData["ErrorMessage"] = "Amendment Type not found.";
                return RedirectToAction("Index");
            }

            return View("ConfirmDelete", AmendmentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int amendmentTypeId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var AmendmentType = await _amendmentTypeService.GetAmendmentTypeViewModelByIdAsync(amendmentTypeId);
                if (AmendmentType == null)
                {
                    TempData["ErrorMessage"] = "Amendment Type not found.";
                    return RedirectToAction("Index");
                }

                var result = await _amendmentTypeService.SoftDeleteAmendmentTypeAsync(amendmentTypeId, userId);

                TempData["SuccessMessage"] = result ? "Amendment Type successfully deleted." : "Failed to delete AmendmentType.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Amendment Type details.");
                TempData["ErrorMessage"] = "An error occurred while loading Amendment Type detail.";
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
