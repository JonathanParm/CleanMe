using CleanMe.Application.Interfaces;
using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanMe.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only authenticated users can access
    public class AreaController : Controller
    {
        private readonly IAreaService _areaService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AreaController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public AreaController(
            IAreaService areaService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AreaController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _areaService = areaService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? regionName, string? name, int? reportCode, string? isActive,
                string sortColumn = "SequenceOrder", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.RegionName = regionName;
            ViewBag.Name = name;
            ViewBag.ReportCode = reportCode;
            ViewBag.IsActive = isActive;

            try
            {
                var areaList = await _areaService.GetAreaIndexAsync(
                    regionName, name, reportCode, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(areaList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Area list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Areas.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int areaId = 0, int regionId = 0, string? returnUrl = null)
        {
            try
            {
                var model = areaId > 0
                    ? await _areaService.GetAreaViewModelWithAssetLocationsByIdAsync(areaId)
                    : await _areaService.PrepareNewAreaViewModelAsync(regionId);

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Area details.");
                TempData["ErrorMessage"] = "An error occurred while loading Area detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update Area
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AreaViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Check for duplicate Area (excluding current record)
                var duplicateArea = await _areaService.FindDuplicateAreaAsync(model.Name, model.ReportCode, model.areaId);
                if (duplicateArea.Any())
                {
                    //TempData["WarningMessage"] = "A Area with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateArea.Select(s => s.areaId).ToArray();
                    ModelState.AddModelError("Name", "An Area with the same name or report code already exists.");
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }

                // Add new Area
                if (model.areaId == 0)
                {
                    int newareaId = await _areaService.AddAreaAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Area {model.Name} added successfully!";
                }
                else // Update Existing Area
                {
                    var existingArea = await _areaService.GetAreaViewModelByIdAsync(model.areaId);
                    if (existingArea == null)
                    {
                        TempData["ErrorMessage"] = "Area record not found.";
                        if (!string.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Area member.");
                    await _areaService.UpdateAreaAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Area {model.Name} updated successfully!";
                }

                Console.WriteLine("DEBUG: Area saved successfully");
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Area.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /Area/Delete?areaId=123
        public async Task<IActionResult> Delete(int areaId, string? returnUrl = null)
        {
            var area = await _areaService.GetAreaViewModelByIdAsync(areaId);
            if (area == null)
            {
                TempData["ErrorMessage"] = "Area not found.";
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("ConfirmDelete", area);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int areaId, string? returnUrl = null)
        {
            try
            {
                var result = await _areaService.SoftDeleteAreaAsync(areaId, GetCurrentUserId());

                TempData["SuccessMessage"] = result ? "Area successfully deleted." : "Failed to delete Area.";

                // Redirect to previous page
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Area details.");
                TempData["ErrorMessage"] = "An error occurred while loading Area detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
