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
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ClientController> _logger;
        private readonly IErrorLoggingService _errorLoggingService;

        public ClientController(
            IClientService clientService,
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            ILogger<ClientController> logger,
            IErrorLoggingService errorLoggingService)
        {
            _clientService = clientService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _errorLoggingService = errorLoggingService;
        }
        public async Task<IActionResult> Index(
            string? name, string? brand, int? accNo, string? isActive,
            string sortColumn = "Name", string sortOrder = "ASC",
            int pageNumber = 1, int pageSize = 20)
        {
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Name = name;
            ViewBag.Brand = brand;
            ViewBag.AccNo = accNo;
            ViewBag.IsActive = isActive;

            try
            {
                var ClientList = await _clientService.GetClientIndexAsync(
                    name, brand, accNo, isActive,
                    sortColumn, sortOrder, pageNumber, pageSize);

                return View(ClientList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Client list.");
                await _errorLoggingService.LogErrorAsync(ex, HttpContext.Request.Path, User.Identity?.Name ?? "System");

                TempData["ErrorMessage"] = "An error occurred while loading Clients.";
                return RedirectToAction("HandleError", "Error");
            }
        }
        // AddEdit Action (Handles Both Add & Edit)
        public async Task<IActionResult> AddEdit(int? clientId, string? returnUrl = null)
        {
            try
            {
                ClientViewModel model;

                if (clientId.HasValue) // Edit Mode
                {
                    model = await _clientService.GetClientWithContactsByIdAsync(clientId.Value);
                    if (model == null)
                    {
                        return NotFound();
                    }
                }
                else // Create Mode
                {
                    model = new ClientViewModel();
                }

                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "Index" : returnUrl;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Client details.");
                TempData["ErrorMessage"] = "An error occurred while loading Client detail.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // Post: Create or Update Client
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ClientViewModel model, string? returnUrl = null)
        {
            try
            {
                Console.WriteLine("DEBUG: Entering AddEdit method.");
                int contactClientId = 0;

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please fix the errors below.";
                    return View(model);
                }

                // Check for duplicate Client (excluding current record)
                var duplicateClient = await _clientService.FindDuplicateClientAsync(model.Name, model.clientId);
                if (duplicateClient.Any())
                {
                    //TempData["WarningMessage"] = "A Client with the same name already exists.";
                    //TempData["MatchingStaffIds"] = duplicateClient.Select(s => s.clientId).ToArray();
                    ModelState.AddModelError("Name", "A Client with the same name already exists.");
                    return View(model);
                }

                // Add new Client
                if (model.clientId == 0)
                {
                    // contact clientId = new clientId
                    contactClientId = await _clientService.AddClientAsync(model, GetCurrentUserId());
                }
                else // Update Existing Client
                {
                    var existingClient = await _clientService.GetClientByIdAsync(model.clientId);
                    if (existingClient == null)
                    {
                        TempData["ErrorMessage"] = "Client record not found.";
                        return RedirectToAction("Index");
                    }
                    contactClientId = existingClient.clientId;

                    Console.WriteLine("DEBUG: Updating existing Client member.");
                    await _clientService.UpdateClientAsync(model, GetCurrentUserId());
                    TempData["SuccessMessage"] = $"Client {model.Name} updated successfully!";
                }

                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                Console.WriteLine("DEBUG: Returning RedirectToAction('Index').");
                return RedirectToAction("Index"); // This is always reached

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding or editing this Client.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("HandleError", "Error");
            }
        }

        // GET: /Client/Delete?ClientId=123
        public async Task<IActionResult> Delete(int clientId)
        {
            var Client = await _clientService.GetClientByIdAsync(clientId);
            if (Client == null)
            {
                TempData["ErrorMessage"] = "Client not found.";
                return RedirectToAction("Index");
            }

            return View("ConfirmDelete", Client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int clientId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var Client = await _clientService.GetClientByIdAsync(clientId);
                if (Client == null)
                {
                    TempData["ErrorMessage"] = "Client not found.";
                    return RedirectToAction("Index");
                }

                var result = await _clientService.SoftDeleteClientAsync(clientId, userId);

                TempData["SuccessMessage"] = result ? "Client successfully deleted." : "Failed to delete Client.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Client details.");
                TempData["ErrorMessage"] = "An error occurred while loading Client detail.";
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
