using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int clientId);
        Task<Client?> GetClientWithContactsByIdAsync(int clientId);
        Task AddClientAsync(Client Client);
        Task UpdateClientAsync(Client Client);
    }
}