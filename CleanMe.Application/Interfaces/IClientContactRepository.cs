using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IClientContactRepository
    {
        // Executes stored procedure via Dapper
        Task<IEnumerable<ClientContactIndexViewModel>> GetClientContactIndexAsync(
            string? clientName, string? fullName, string? jobTitle, string? phoneMobile, string? email, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);


        Task<ClientContact?> GetClientContactByIdAsync(int clientContactId);
        Task<ClientContactViewModel?> GetClientContactViewModelByIdAsync(int clientContactId);
        Task<ClientContactViewModel> PrepareNewClientContactViewModelAsync(int clientId);

        Task<bool> IsEmailAvailableAsync(string email, int clientContactId);

        Task AddClientContactAsync(ClientContact clientContact, string createdById);
        Task UpdateClientContactAsync(ClientContact clientContact, string updatedById);
        Task SoftDeleteClientContactAsync(int clientContactId, string deletedById);
    }
}
