// --- Namespace: CleanMe.Infrastructure.Repositories ---
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CleanMe.Infrastructure.Repositories
{
    public class ClientContactRepository : IClientContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClientContact>> GetAllClientContactsAsync()
        {
            return await _context.ClientContacts
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.FamilyName)
                .ToListAsync();
        }

        public async Task<ClientContact?> GetClientContactByIdAsync(int clientContactId)
        {
            return await _context.ClientContacts
                .Include(c => c.Client)  // eager load
                .FirstOrDefaultAsync(cc => cc.clientContactId == clientContactId);
        }

        public async Task AddClientContactAsync(ClientContact clientContact)
        {
            await _context.ClientContacts.AddAsync(clientContact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientContactAsync(ClientContact clientContact)
        {
            _context.ClientContacts.Update(clientContact);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsEmailAvailableAsync(string email, int clientContactId)
        {
            return !await _context.ClientContacts
                .Where(c => !c.IsDeleted && c.Email == email &&
                       (clientContactId == 0 || c.clientContactId != clientContactId))
                .AnyAsync();
        }
    }
}
