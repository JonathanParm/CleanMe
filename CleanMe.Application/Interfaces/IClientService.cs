using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientIndexViewModel>> GetClientIndexAsync(
            string? name, string? brand, int? accNo, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<ClientViewModel>> FindDuplicateClientAsync(string name, int? excludeClientId);
        Task<ClientViewModel?> GetClientViewModelByIdAsync(int clientId);
        Task<ClientViewModel?> GetClientViewModelWithContactsByIdAsync(int clientId);
        Task<int> AddClientAsync(ClientViewModel model, string addedById);
        Task UpdateClientAsync(ClientViewModel model, string updatedById);
        Task<bool> SoftDeleteClientAsync(int clientId, string updatedById);
    }
}