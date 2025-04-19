// --- Namespace: CleanMe.Infrastructure.Repositories ---
using CleanMe.Application.DTOs;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Common;
using CleanMe.Domain.Entities;
using CleanMe.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CleanMe.Infrastructure.Repositories
{
    public class ClientContactRepository : IClientContactRepository
    {
        private readonly IDapperRepository _dapper;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public ClientContactRepository(IConfiguration configuration, ApplicationDbContext context, IDapperRepository dapper)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _context = context;
            _dapper = dapper;
        }

        public async Task<IEnumerable<ClientContactIndexViewModel>> GetClientContactIndexAsync(
            string? clientName, string? fullName, string? jobTitle, string? phoneMobile, string? email, string? isActive,
            string sortColumn, string sortOrder,
            int pageNumber, int pageSize)
        {
            var sql = "EXEC dbo.ClientContactGetIndexView @ClientName, @FullName, @JobTitle, @phoneMobile, @email, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
            var parameters = new
            {
                ClientName = clientName,
                FullName = fullName,
                JobTitle = jobTitle,
                PhoneMobile = phoneMobile,
                Email = email,
                IsActive = isActive,
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ClientContactIndexViewModel>(sql, parameters);
        }

        public async Task<ClientContact?> GetClientContactByIdAsync(int clientContactId)
        {
            return await _context.ClientContacts.FindAsync(clientContactId);
        }

        public async Task<ClientContactViewModel?> GetClientContactViewModelByIdAsync(int clientContactId)
        {
            return await _context.ClientContacts
                .Where(cc => cc.clientContactId == clientContactId)
                .Select(cc => new ClientContactViewModel
                {
                    clientContactId = cc.clientContactId,
                    clientId = cc.clientId,
                    FirstName = cc.FirstName,
                    FamilyName = cc.FamilyName,
                    Email = cc.Email,
                    PhoneMobile = cc.PhoneMobile,
                    JobTitle = cc.JobTitle,
                    IsActive = cc.IsActive,
                    ApplicationUserId = cc.ApplicationUserId,
                    ClientName = cc.Client.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ClientContactViewModel> PrepareNewClientContactViewModelAsync(int clientId)
        {
            var client = await _context.Clients
                .Where(c => c.clientId == clientId && !c.IsDeleted)
                .Select(c => new { c.clientId, c.Name })
                .FirstOrDefaultAsync();

            //if (client == null)
            //    throw new NotFoundException("Client not found.");

            return new ClientContactViewModel
            {
                clientId = client.clientId,
                ClientName = client.Name,
                IsActive = true
            };
        }

        public async Task<bool> IsEmailAvailableAsync(string email, int clientContactId)
        {
            return !await _context.ClientContacts
                .Where(c => !c.IsDeleted && c.Email == email &&
                       (clientContactId == 0 || c.clientContactId != clientContactId))
                .AnyAsync();
        }


        public async Task AddClientContactAsync(ClientContact clientContact, string createdById)
        {
            await _context.ClientContacts.AddAsync(clientContact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientContactAsync(ClientContact clientContact, string updatedById)
        {
            _context.ClientContacts.Update(clientContact);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteClientContactAsync(int id, string deletedById)
        {
            var clientContact = await _context.ClientContacts.FindAsync(id);
            if (clientContact != null)
            {
                _context.ClientContacts.Remove(clientContact);
                await _context.SaveChangesAsync();
            }
        }

    }
}
