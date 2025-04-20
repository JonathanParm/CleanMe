using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CleanMe.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int clientId)
        {
            return await _context.Clients.FindAsync(clientId);
        }

        public async Task<Client?> GetClientWithContactsByIdAsync(int clientId)
        {
            return await _context.Clients
                .Include(c => c.ClientContacts.Where(cc => !cc.IsDeleted))
                .FirstOrDefaultAsync(c => c.clientId == clientId);
        }

        public async Task AddClientAsync(Client Client)
        {
            await _context.Clients.AddAsync(Client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientAsync(Client Client)
        {
            _context.Clients.Update(Client);
            await _context.SaveChangesAsync();
        }
    }
}
