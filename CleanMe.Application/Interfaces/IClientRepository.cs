using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<ClientIndexViewModel>> GetClientIndexAsync(
                string? name, string? brand, int? accNo, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<Client?> GetClientByIdAsync(int clientId);
        Task<Client?> GetClientWithContactsByIdAsync(int clientId);
        Task AddClientAsync(Client Client, string createdById);
        Task UpdateClientAsync(Client Client, string updatedById);
        Task SoftDeleteClientAsync(int id, string deletedById);
    }
}