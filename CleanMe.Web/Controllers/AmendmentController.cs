using CleanMe.Application.Filters;
using CleanMe.Application.Interfaces;
using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using CleanMe.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMe.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only authenticated users can access
    public class AmendmentController : Controller
    {
        private readonly IAmendmentService _amendmentService;
        private readonly ILookupService _lookupService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AmendmentController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public AmendmentController(
            IAmendmentService amendmentService,
            ILookupService lookupService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<AmendmentController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _amendmentService = amendmentService;
            _lookupService = lookupService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
                string? sourceName, string? amendTypeName,
                string? clientName, string? areaName, string? locationName,
                string? mdReference, string? clientReference, string sortColumn = "SourceName", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SourceName = sourceName;
            ViewBag.AmendTypeName = amendTypeName;
            ViewBag.ClientName = clientName;
            ViewBag.AreaName = areaName;
            ViewBag.LocationName = locationName;
            ViewBag.MDReference = mdReference;
            ViewBag.ClientReference = clientReference;

            try
            {
                var amendmentList = await _amendmentService.GetAmendmentIndexAsync(
                    sourceName, amendTypeName,
                    clientName, areaName, locationName,
                    mdReference, clientReference,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(amendmentList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Amendment list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Amendments.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int? amendmentId, string? returnUrl = null)
        {
            try
            {
                AmendmentViewModel modelCurrent;

                if (amendmentId.HasValue) // Edit Mode
                {
                    modelCurrent = await _amendmentService.GetAmendmentViewModelByIdAsync(amendmentId.Value);
                    if (modelCurrent == null)
                    {
                        return NotFound();
                    }
                    modelCurrent.amendmentId = amendmentId.Value;

                }
                else // Create Mode
                {
                    modelCurrent = new AmendmentViewModel();
                }

                AmendmentAddEditViewModel model = new AmendmentAddEditViewModel();
                model.amendmentId = amendmentId;
                model.AmendmentCurrent = modelCurrent;

                if (modelCurrent.amendmentTypeId.HasValue)
                {
                    var amendmentTypeId = modelCurrent.amendmentTypeId.Value;
                    var amendmentLastInvoicedId = await _amendmentService.GetAmendmentLastInvoicedOnByIdAsync(amendmentId.Value, amendmentTypeId);
                    if (amendmentLastInvoicedId.HasValue)
                    {
                        model.AmendmentLastInvoiced = await _amendmentService.GetAmendmentViewModelByIdAsync(amendmentId.Value);
                    }
                }

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;
                await PopulateSelectListsAsync(model, model.amendmentId > 0);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Amendment details.");
                TempData["ErrorMessage"] = "An error occurred while loading Amendment detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update Amendment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AmendmentAddEditViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    await PopulateSelectListsAsync(model, model.amendmentId > 0);

                    return View(model);
                }

                // Check for duplicate Amendment (excluding current record)
                var duplicateAmendment = await _amendmentService.FindDuplicateAmendmentAsync(
                        model.AmendmentCurrent.amendmentTypeId.Value,
                        model.AmendmentCurrent.assetId.Value,
                        model.AmendmentCurrent.amendmentId
                    );
                if (duplicateAmendment.Any())
                {
                    //TempData["WarningMessage"] = "An Amendment Type with the same name or code already exists.";
                    //TempData["MatchingStaffIds"] = duplicateAmendment.Select(s => s.amendmentId).ToArray();
                    ModelState.AddModelError("Name", "An Amendment for this amendment type already exists.");
                    await PopulateSelectListsAsync(model, model.amendmentId > 0);

                    return View(model);
                }

                // Add new Amendment
                if (model.amendmentId == 0)
                {
                    int newamendmentId = await _amendmentService.AddAmendmentAsync(model.AmendmentCurrent, GetCurrentUserId());
                }
                else // Update Existing Amendment
                {
                    var existingAmendment = await _amendmentService.GetAmendmentViewModelByIdAsync(model.AmendmentCurrent.amendmentId.Value);
                    if (existingAmendment == null)
                    {
                        TempData["ErrorMessage"] = "Amendment record not found.";
                        return RedirectToAction("Index");
                    }

                    Console.WriteLine("DEBUG: Updating existing Amendment.");
                    await _amendmentService.UpdateAmendmentAsync(model.AmendmentCurrent, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Amendment {model.AmendmentCurrent.AmendmentTypeName} updated successfully!";
                }

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                Console.WriteLine("DEBUG: Returning RedirectToAction('Index').");
                return RedirectToAction("Index"); // This is always reached

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Amendment.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /Amendment/Delete?amendmentId=123
        public async Task<IActionResult> Delete(int amendmentId)
        {
            var Amendment = await _amendmentService.GetAmendmentViewModelByIdAsync(amendmentId);
            if (Amendment == null)
            {
                TempData["ErrorMessage"] = "Amendment not found.";
                return RedirectToAction("Index");
            }

            return View("ConfirmDelete", Amendment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int amendmentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var Amendment = await _amendmentService.GetAmendmentViewModelByIdAsync(amendmentId);
                if (Amendment == null)
                {
                    TempData["ErrorMessage"] = "Amendment not found.";
                    return RedirectToAction("Index");
                }

                var result = await _amendmentService.SoftDeleteAmendmentAsync(amendmentId, userId);

                TempData["SuccessMessage"] = result ? "Amendment successfully deleted." : "Failed to delete Amendment.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Amendment details.");
                TempData["ErrorMessage"] = "An error occurred while loading Amendment detail.";
                return RedirectToAction("HandleError", "Error");
            }

            return RedirectToAction("Index");
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task PopulateSelectListsAsync(AmendmentAddEditViewModel model, bool isEditMode)
        {
            if (isEditMode)
            {
                model.AmendmentTypes = await _lookupService.GetAmendmentTypeSelectListAsync();
                model.Areas = await _lookupService.GetAreaSelectListAsync(new AreaLookupFilter());
                model.AssetLocations = await _lookupService.GetAssetLocationSelectListAsync(new AssetLocationLookupFilter());
                model.Assets = await _lookupService.GetAssetSelectListAsync(new AssetLookupFilter());
                model.CleanFrequencies = await _lookupService.GetCleanFrequencySelectListAsync();
                model.Clients = await _lookupService.GetClientSelectListAsync();
                model.Staff = await _lookupService.GetStaffSelectListAsync(new StaffLookupFilter());
            }
            else
            {
                model.AmendmentTypes = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select amendment type --" }
                }.Concat(await _lookupService.GetAmendmentTypeSelectListAsync());

                model.Areas = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select area --" }
                }.Concat(await _lookupService.GetAreaSelectListAsync(new AreaLookupFilter()));

                model.AssetLocations = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select location --" }
                }.Concat(await _lookupService.GetAssetLocationSelectListAsync(new AssetLocationLookupFilter()));

                model.Assets = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select asset --" }
                }.Concat(await _lookupService.GetAssetSelectListAsync(new AssetLookupFilter()));

                model.CleanFrequencies = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select clean frequency --" }
                }.Concat(await _lookupService.GetCleanFrequencySelectListAsync());

                model.Clients = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select client --" }
                }.Concat(await _lookupService.GetClientSelectListAsync());


                model.Staff = new[]
                {
                    new SelectListItem { Value = "", Text = "-- Select staff --" }
                }.Concat(await _lookupService.GetStaffSelectListAsync(new StaffLookupFilter()));
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAreas(int clientId)
        {
            var areaLookupFilter = new AreaLookupFilter
            {
                StaffId = 0,
                ClientId = clientId,
                RegionId = 0,
            };

            var locations = await _lookupService.GetAreaSelectListAsync(areaLookupFilter);
            return Json(locations);
        }

        [HttpGet]
        public async Task<JsonResult> GetAssetLocations(int clientId, int? areaId = 0)
        {
            var assetLocationLookupFilter = new AssetLocationLookupFilter
            {
                StaffId = 0,
                ClientId = clientId,
                RegionId = 0,
                AreaId = areaId
            };

            var locations = await _lookupService.GetAssetLocationSelectListAsync(assetLocationLookupFilter);
            return Json(locations);
        }

        [HttpGet]
        public async Task<JsonResult> GetAssets(int clientId, int? areaId = 0, int? assetLocationId = 0)
        {
            var assetLookupFilter = new AssetLookupFilter
            {
                StaffId = 0,
                ClientId = clientId,
                RegionId = 0,
                AreaId = areaId,
                AssetLocationId = assetLocationId
            };

            var assets = await _lookupService.GetAssetSelectListAsync(assetLookupFilter);
            return Json(assets);
        }
    }
}
