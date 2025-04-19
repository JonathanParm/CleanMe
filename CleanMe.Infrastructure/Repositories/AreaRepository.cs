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
    public class AreaRepository : IAreaRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IDapperRepository _dapperRepository;

        public AreaRepository(IConfiguration configuration, ApplicationDbContext context, IDapperRepository dapperRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<AreaIndexViewModel>> GetAreaIndexAsync(
                string? regionName, string? name, int? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            try
            {
                var sql = "EXEC dbo.AreaGetIndexView @RegionName, @Name, @ReportCode, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    RegionName = regionName,
                    Name = name,
                    ReportCode = reportCode,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                using var connection = new SqlConnection(_connectionString);
                return await connection.QueryAsync<AreaIndexViewModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Areas from stored procedure", ex);
            }
        }

        public async Task<AreaViewModel?> GetAreaByIdAsync(int areaId)
        {
            var area = await _context.Areas.FindAsync(areaId);

            if (area == null)
            {
                return null; // No match found
            }

            // Convert `Area` entity to `AreaViewModel`
            return new AreaViewModel
            {
                areaId = area.areaId,
                Name = area.Name,
                regionId = area.regionId,
                ReportCode = area.ReportCode,
                SequenceOrder = area.SequenceOrder,
                SeqNo = area.SeqNo,
                IsActive = area.IsActive
            };
        }

        public async Task<AreaViewModel?> GetAreaViewModelByIdAsync(int areaId)
        {
            var area = await _context.Areas
                .Include(a => a.Region)
                .FirstOrDefaultAsync(a => a.areaId == areaId);

            if (area == null)
            {
                return null; // No match found
            }

            // Convert `Area` entity to `AreaViewModel`
            return new AreaViewModel
            {
                areaId = area.areaId,
                Name = area.Name,
                regionId = area.regionId,
                RegionName = area.Region?.Name,
                SequenceOrder = area.SequenceOrder,
                SeqNo = area.SeqNo,
                ReportCode = area.ReportCode,
                IsActive = area.IsActive
            };
        }

        public async Task<Area?> GetAreaViewModelWithAssetLocationsByIdAsync(int areaId)
        {
            return await _context.Areas
                .Include(a => a.Region) // parent region
                .Include(a => a.AssetLocations.Where(al => !al.IsDeleted))
                .FirstOrDefaultAsync(a => a.areaId == areaId);
        }

        public async Task<AreaViewModel> PrepareNewAreaViewModelAsync(int regionId)
        {
            var region = await _context.Regions
                .Where(r => r.regionId == regionId)
                .Select(r => new { r.regionId, r.Name })
                .FirstOrDefaultAsync();

            //if (region == null)
            //    throw new NotFoundException("Region not found.");

            return new AreaViewModel
            {
                regionId = region.regionId,
                RegionName = region.Name,
                AssetLocationsList = new List<AssetLocationIndexViewModel>(),
                IsActive = true
            };
        }

        public async Task AddAreaAsync(Area Area, string createdById)
        {
            await _context.Areas.AddAsync(Area);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAreaAsync(Area Area, string updatedById)
        {
            _context.Areas.Update(Area);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAreaAsync(int id, string deletedById)
        {
            var Area = await _context.Areas.FindAsync(id);
            if (Area != null)
            {
                _context.Areas.Remove(Area);
                await _context.SaveChangesAsync();
            }
        }
    }
}
