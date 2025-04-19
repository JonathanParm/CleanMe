// This code shows best-practice structure for the CleanMe MVC project, applying UnitOfWork and separation of concerns

// --- Namespace: CleanMe.Web.Controllers ---
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CleanMe.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClientContactController : Controller
    {
        private readonly IClientContactService _clientContactService;
        private readonly IUserService _userService;
        private readonly ILogger<ClientContactController> _logger;
        private readonly IErrorLoggingService _errorLogger;

        public ClientContactController(
            IClientContactService clientContactService,
            IUserService userService,
            ILogger<ClientContactController> logger,
            IErrorLoggingService errorLogger)
        {
            _clientContactService = clientContactService;
            _userService = userService;
            _logger = logger;
            _errorLogger = errorLogger;
        }

        public async Task<IActionResult> Index(
            string? clientName, string? fullName, string? jobTitle,
            string? phoneMobile, string? email, string? isActive,
            string sortColumn = "ClientContactNo", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;

            try
            {
                var viewModel = await _clientContactService.GetClientContactIndexAsync(
                    clientName, fullName, jobTitle, phoneMobile, email, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                TempData["ErrorMessage"] = "Failed to load Client Contact list.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        public async Task<IActionResult> AddEdit(int clientContactId = 0, int clientId = 0, string? returnUrl = null)
        {
            try
            {
                var model = clientContactId > 0
                    ? await _clientContactService.GetClientContactViewModelByIdAsync(clientContactId)
                    : await _clientContactService.PrepareNewClientContactViewModelAsync(clientId);

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                TempData["ErrorMessage"] = "Failed to load Client Contact detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ClientContactViewModel model, string? returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid || !ValidateClientContact(model))
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                if (!await ValidateEmailUniqueness(model))
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                var userId = GetCurrentUserId();

                if (model.clientContactId == 0)
                    model.clientContactId = await _clientContactService.AddClientContactAsync(model, userId);
                else
                    await _clientContactService.UpdateClientContactAsync(model, userId);

                if (model.ShowPasswordSection)
                    await HandleLoginAsync(model);

                TempData["SuccessMessage"] = "Client Contact saved successfully.";
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                TempData["ErrorMessage"] = "Failed to save Client Contact.";

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int clientContactId, string? returnUrl = null)
        {
            try
            {
                var clientContact = await _clientContactService.GetClientContactByIdAsync(clientContactId);
                if (clientContact == null)
                {
                    TempData["ErrorMessage"] = "Client contact not found.";
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index");
                }

                ViewBag.ReturnUrl = returnUrl;
                return View("ConfirmDelete", clientContact);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                return RedirectToAction("HandleError", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int clientContactId, string? returnUrl = null)
        {
            try
            {
                var result = await _clientContactService.SoftDeleteClientContactAsync(clientContactId, GetCurrentUserId());
                if (result)
                {
                    var clientContact = await _clientContactService.GetClientContactByIdAsync(clientContactId);
                    if (!string.IsNullOrWhiteSpace(clientContact?.ApplicationUserId))
                        await _userService.DisableUserLoginAsync(clientContact.ApplicationUserId);
                }

                TempData["SuccessMessage"] = "Client Contact deleted.";

                // Redirect to previous page
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                TempData["ErrorMessage"] = "Failed to delete Client Contact.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailAvailable(string email, int clientContactId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Json(true);

            bool isAvailable = await _clientContactService.IsEmailAvailableAsync(email, clientContactId);
            return Json(isAvailable);
        }

        private async Task<bool> ValidateEmailUniqueness(ClientContactViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return true;

            try
            {
                bool isAvailable = await _clientContactService.IsEmailAvailableAsync(model.Email, model.clientContactId);
                if (!isAvailable)
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await LogErrorAsync(ex);
                ModelState.AddModelError("Email", "Could not validate Client Contact email uniqueness.");
                return false;
            }

            return true;
        }

        private bool ValidateClientContact(ClientContactViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.PhoneMobile)
                    && string.IsNullOrWhiteSpace(model.Email))
                {
                    ModelState.AddModelError("", "At least one contact method is required.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogErrorAsync(ex);
                ModelState.AddModelError("", "Failed to validate Client Contact.");
                return false;
            }
            return true;
        }

        private async Task HandleLoginAsync(ClientContactViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ApplicationUserId))
            {
                var (result, userId) = await _userService.CreateUserLoginAsync(model.Email, model.ChangePassword.Password);
                if (result.Succeeded)
                    await _clientContactService.UpdateClientContactApplicationUserId(model.clientContactId, userId);
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