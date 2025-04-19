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
    public class RegionRepository : IRegionRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IDapperRepository _dapperRepository;

        public RegionRepository(IConfiguration configuration, ApplicationDbContext context, IDapperRepository dapperRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<RegionIndexViewModel>> GetRegionIndexAsync(
                string? name, string? code, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            try
            {
                var sql = "EXEC dbo.RegionGetIndexView @Name, @Code, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    Code = code,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                using var connection = new SqlConnection(_connectionString);
                return await connection.QueryAsync<RegionIndexViewModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching regions from stored procedure", ex);
            }
        }

        public async Task<Region?> GetRegionByIdAsync(int regionId)
        {
            return await _context.Regions.FindAsync(regionId);
        }

        public async Task<Region?> GetRegionWithAreasByIdAsync(int regionId)
        {
            return await _context.Regions
                .Include(r => r.Areas.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(r => r.regionId == regionId);
        }

        public async Task AddRegionAsync(Region region, string createdById)
        {
            await _context.Regions.AddAsync(region);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRegionAsync(Region region, string updatedById)
        {
            _context.Regions.Update(region);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteRegionAsync(int id, string deletedById)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region != null)
            {
                _context.Regions.Remove(region);
                await _context.SaveChangesAsync();
            }
        }
    }
}
