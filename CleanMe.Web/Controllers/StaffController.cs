// This code shows best-practice structure for the CleanMe MVC project, applying UnitOfWork and separation of concerns

// --- Namespace: CleanMe.Web.Controllers ---
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using Microsoft.Extensions.Logging;

namespace CleanMe.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly IUserService _userService;
        private readonly ILogger<StaffController> _logger;
        private readonly IErrorLoggingService _errorLogger;

        public StaffController(
            IStaffService staffService,
            IUserService userService,
            ILogger<StaffController> logger,
            IErrorLoggingService errorLogger)
        {
            _staffService = staffService;
            _userService = userService;
            _logger = logger;
            _errorLogger = errorLogger;
        }

        public async Task<IActionResult> Index(
            string? staffNo, string? fullName, string? workRole,
            string? contactDetail, string? isActive,
            string sortColumn = "StaffNo", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;

            try
            {
                var viewModel = await _staffService.GetStaffIndexAsync(
                    staffNo, fullName, workRole, contactDetail, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                TempData["ErrorMessage"] = "Failed to load staff list.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        public async Task<IActionResult> AddEdit(int? staffId)
        {
            try
            {
                var model = staffId.HasValue
                    ? await _staffService.GetStaffByIdAsync(staffId.Value)
                    : new StaffViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                TempData["ErrorMessage"] = "Failed to load staff detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(StaffViewModel model)
        {
            try
            {
                if (!ModelState.IsValid || !ValidateContact(model))
                    return View(model);

                if (!await ValidateEmailUniqueness(model))
                    return View(model);

                var userId = GetCurrentUserId();

                if (model.StaffId == 0)
                    model.StaffId = await _staffService.AddStaffAsync(model, userId);
                else
                    await _staffService.UpdateStaffAsync(model, userId);

                if (model.ShowPasswordSection)
                    await HandleLoginAsync(model);

                TempData["SuccessMessage"] = "Staff saved successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                TempData["ErrorMessage"] = "Failed to save staff.";
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int staffId)
        {
            try
            {
                var staff = await _staffService.GetStaffByIdAsync(staffId);
                if (staff == null) return RedirectToAction("Index");
                return View("ConfirmDelete", staff);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                return RedirectToAction("HandleError", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int staffId)
        {
            try
            {
                var result = await _staffService.SoftDeleteStaffAsync(staffId, GetCurrentUserId());
                if (result)
                {
                    var staff = await _staffService.GetStaffByIdAsync(staffId);
                    if (!string.IsNullOrWhiteSpace(staff?.ApplicationUserId))
                        await _userService.DisableUserLoginAsync(staff.ApplicationUserId);
                }

                TempData["SuccessMessage"] = "Staff deleted.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                TempData["ErrorMessage"] = "Failed to delete staff.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        private async Task<bool> ValidateEmailUniqueness(StaffViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                bool isAvailable = await _staffService.IsEmailAvailableAsync(model.Email, model.StaffId);
                if (!isAvailable)
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return false;
                }
            }
            return true;
        }

        private bool ValidateContact(StaffViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.PhoneHome)
                && string.IsNullOrWhiteSpace(model.PhoneMobile)
                && string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError("", "At least one contact method is required.");
                return false;
            }
            return true;
        }

        private async Task HandleLoginAsync(StaffViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ApplicationUserId))
            {
                var (result, userId) = await _userService.CreateUserLoginAsync(model.Email, model.ChangePassword.Password);
                if (result.Succeeded)
                    await _staffService.UpdateStaffApplicationUserId(model.StaffId, userId);
                else
                    TempData["ErrorMessage"] = string.Join(", ", result.Errors.Select(e => e.Description));
            }
            else
            {
                var updateResult = await _userService.UpdateUserLoginAsync(
                    model.ApplicationUserId, model.Email, model.ChangePassword.Password);

                if (!updateResult.Succeeded)
                    TempData["ErrorMessage"] = "Failed to update user login.";
            }
        }

        private async Task LogErrorAsync(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await _errorLogger.LogErrorAsync(ex, HttpContext.Request.Path, GetCurrentUserId());
        }
    }
}