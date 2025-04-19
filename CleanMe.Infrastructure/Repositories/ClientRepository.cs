using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Enums;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IDapperRepository _dapperRepository;

        public ClientRepository(IConfiguration configuration, ApplicationDbContext context, IDapperRepository dapperRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<ClientIndexViewModel>> GetClientIndexAsync(
                string? name, string? brand, int? accNo, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var sql = "EXEC dbo.ClientGetIndexView @Name, @Brand, @AccNo, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    Brand = brand,
                    AccNo = accNo,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await connection.QueryAsync<ClientIndexViewModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Clients from stored procedure", ex);
            }
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

        public async Task AddClientAsync(Client Client, string createdById)
        {
            await _context.Clients.AddAsync(Client);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClientAsync(Client Client, string updatedById)
        {
            _context.Clients.Update(Client);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteClientAsync(int id, string deletedById)
        {
            var Client = await _context.Clients.FindAsync(id);
            if (Client != null)
            {
                _context.Clients.Remove(Client);
                await _context.SaveChangesAsync();
            }
        }
    }
}
