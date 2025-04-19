using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientViewModel>> FindDuplicateClientAsync(string name, int? excludeClientId);

        Task<ClientViewModel?> GetClientByIdAsync(int clientId);
        Task<ClientViewModel?> GetClientWithContactsByIdAsync(int clientId);
        // Creates a new Client using EF Core
        Task<int> AddClientAsync(ClientViewModel model, string addedById);
        // Updates an existing Client using EF Core
        Task UpdateClientAsync(ClientViewModel model, string updatedById);
        Task<bool> SoftDeleteClientAsync(int clientId, string updatedById);

        // Retrieves a paginated & filtered Client list using Dapper
        Task<IEnumerable<ClientIndexViewModel>> GetClientIndexAsync(
            string? name,
            string? brand,
            int? accNo,
            string? isActive,
            string sortColumn,
            string sortOrder,
            int pageNumber,
            int pageSize);
    }
}