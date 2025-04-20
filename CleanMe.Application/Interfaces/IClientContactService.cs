using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IClientContactService
    {
        Task<IEnumerable<ClientContactIndexViewModel>> GetClientContactIndexAsync(
            string? clientName, string? fullName, string? jobTitle, string? phoneMobile, string? email, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<ClientContactViewModel>> FindDuplicateClientContactAsync(string firstName, string familyName, int? excludeClientContactId);
        Task<ClientContactViewModel?> GetClientContactViewModelByIdAsync(int clientContactId);
        Task<ClientContactViewModel> PrepareNewClientContactViewModelAsync(int clientId);
        Task<int> AddClientContactAsync(ClientContactViewModel model, string addedById);
        Task UpdateClientContactAsync(ClientContactViewModel model, string updatedById);
        Task<bool> SoftDeleteClientContactAsync(int clientContactId, string updatedById);
        Task<bool> IsEmailAvailableAsync(string email, int clientContactId);
        Task UpdateClientContactApplicationUserId(int clientContactId, string applicationUserId);
    }
}