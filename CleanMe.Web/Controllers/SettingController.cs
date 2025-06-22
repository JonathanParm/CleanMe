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
    public class SettingController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AssetController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public SettingController(
            ISettingService settingService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AssetController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _settingService = settingService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Edit()
        {
            try
            {
                var model = await _settingService.GetSettingViewModelAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Setting.");
                TempData["ErrorMessage"] = "An error occurred while loading Company Information.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SettingViewModel model)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering Edit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    return View(model);
                }

                    var existingSetting = await _settingService.GetSettingViewModelAsync();
                    if (existingSetting == null)
                    {
                        TempData["ErrorMessage"] = "System setting record not found.";

                    return RedirectToAction("Index", "Home");
                }

                    Console.WriteLine("DEBUG: Updating existing system setting.");
                    var result = await _settingService.UpdateSettingAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"System setting updated successfully!";

                Console.WriteLine("DEBUG: System setting saved successfully");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing system setting.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

    }
}
