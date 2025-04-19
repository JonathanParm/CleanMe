using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Enums;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Infrastructure.Repositories
{
    public class CleanFrequencyRepository : ICleanFrequencyRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IDapperRepository _dapperRepository;

        public CleanFrequencyRepository(IConfiguration configuration, ApplicationDbContext context, IDapperRepository dapperRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<CleanFrequencyIndexViewModel>> GetCleanFrequencyIndexAsync(
                string? name, string? description, string? code, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var sql = "EXEC dbo.CleanFrequencyGetIndexView @Name, @Description, @Code, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    Description = description,
                    Code = code,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await connection.QueryAsync<CleanFrequencyIndexViewModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching CleanFrequencys from stored procedure", ex);
            }
        }

        public async Task<CleanFrequency?> GetCleanFrequencyByIdAsync(int id)
        {
            return await _context.CleanFrequencies.FindAsync(id);
        }

        public async Task AddCleanFrequencyAsync(CleanFrequency CleanFrequency, string createdById)
        {
            await _context.CleanFrequencies.AddAsync(CleanFrequency);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCleanFrequencyAsync(CleanFrequency CleanFrequency, string updatedById)
        {
            _context.CleanFrequencies.Update(CleanFrequency);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteCleanFrequencyAsync(int id, string deletedById)
        {
            var CleanFrequency = await _context.CleanFrequencies.FindAsync(id);
            if (CleanFrequency != null)
            {
                _context.CleanFrequencies.Remove(CleanFrequency);
                await _context.SaveChangesAsync();
            }
        }
    }
}
