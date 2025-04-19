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
    public class AssetTypeRepository : IAssetTypeRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IDapperRepository _dapperRepository;

        public AssetTypeRepository(IConfiguration configuration, ApplicationDbContext context, IDapperRepository dapperRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<AssetTypeIndexViewModel>> GetAssetTypeIndexAsync(
                string? name, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var sql = "EXEC dbo.AssetTypeGetIndexView @Name, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await connection.QueryAsync<AssetTypeIndexViewModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Asset Types from stored procedure", ex);
            }
        }

        public async Task<AssetType?> GetAssetTypeByIdAsync(int id)
        {
            return await _context.AssetTypes.FindAsync(id);
        }

        public async Task AddAssetTypeAsync(AssetType AssetType, string createdById)
        {
            await _context.AssetTypes.AddAsync(AssetType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssetTypeAsync(AssetType AssetType, string updatedById)
        {
            _context.AssetTypes.Update(AssetType);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAssetTypeAsync(int id, string deletedById)
        {
            var AssetType = await _context.AssetTypes.FindAsync(id);
            if (AssetType != null)
            {
                _context.AssetTypes.Remove(AssetType);
                await _context.SaveChangesAsync();
            }
        }
    }
}
