using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IClientContactService
    {
        Task<IEnumerable<ClientContactViewModel>> FindDuplicateClientContactAsync(string firstName, string familyName, int? excludeClientContactId);
        Task<bool> IsEmailAvailableAsync(string email, int clientContactId);

        Task<ClientContact?> GetClientContactByIdAsync(int clientContactId);
        Task<ClientContactViewModel?> GetClientContactViewModelByIdAsync(int clientContactId);
        Task<ClientContactViewModel> PrepareNewClientContactViewModelAsync(int clientId);

        // Creates a new ClientContact using EF Core
        Task<int> AddClientContactAsync(ClientContactViewModel model, string addedById);
        // Updates an existing ClientContact using EF Core
        Task UpdateClientContactAsync(ClientContactViewModel model, string updatedById);
        Task UpdateClientContactApplicationUserId(int clientContactId, string applicationUserId);
        Task<bool> SoftDeleteClientContactAsync(int clientContactId, string updatedById);

        // Retrieves a paginated & filtered ClientContact list using Dapper
        Task<IEnumerable<ClientContactIndexViewModel>> GetClientContactIndexAsync(
            string? clientName, string? fullName, string? jobTitle, string? phoneMobile, string? email, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);
    }
}