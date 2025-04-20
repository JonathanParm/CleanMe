using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IClientContactRepository
    {
        Task<IEnumerable<ClientContact>> GetAllClientContactsAsync();
        Task<ClientContact?> GetClientContactByIdAsync(int clientContactId);
        Task AddClientContactAsync(ClientContact clientContact);
        Task UpdateClientContactAsync(ClientContact clientContact);
        Task<bool> IsEmailAvailableAsync(string email, int clientContactId);
    }
}
