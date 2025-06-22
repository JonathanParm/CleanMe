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
    public class CompanyInfoController : Controller
    {
        private readonly ICompanyInfoService _companyInfoService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AssetController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public CompanyInfoController(
            ICompanyInfoService companyInfoService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AssetController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _companyInfoService = companyInfoService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Edit()
        {
            try
            {
                var model = await _companyInfoService.GetCompanyInfoViewModelAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CompanyInfo.");
                TempData["ErrorMessage"] = "An error occurred while loading Company Information.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyInfoViewModel model)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering Edit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    return View(model);
                }

                    var existingCompanyInfo = await _companyInfoService.GetCompanyInfoViewModelAsync();
                    if (existingCompanyInfo == null)
                    {
                        TempData["ErrorMessage"] = "Company info record not found.";

                    return RedirectToAction("Index", "Home");
                }

                    Console.WriteLine("DEBUG: Updating existing Company information.");
                    var result = await _companyInfoService.UpdateCompanyInfoAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Company informantion updated successfully!";

                Console.WriteLine("DEBUG: Company information saved successfully");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing company information.");
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
