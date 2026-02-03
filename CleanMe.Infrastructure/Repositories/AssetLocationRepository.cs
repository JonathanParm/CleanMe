using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CleanMe.Infrastructure.Repositories
{
    public class AssetLocationRepository : IAssetLocationRepository
    {
        private readonly ApplicationDbContext _context;

        public AssetLocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssetLocation>> GetAllAssetLocationsAsync()
        {
            try
            {
                // Using Dapper for a raw SQL query
                using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT * FROM AssetLocations WHERE IsDeleted = 0 ORDER BY Description";
                    return await connection.QueryAsync<AssetLocation>(sql);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving asset locations.", ex);
            }
            //return await _context.AssetLocations
            //    .Where(c => !c.IsDeleted)
            //    .OrderBy(c => c.Description)
            //    .ToListAsync();
        }

        public async Task<AssetLocation?> GetAssetLocationByIdAsync(int assetLocationId)
        {
            return await _context.AssetLocations.FindAsync(assetLocationId);
        }

        public async Task<AssetLocationViewModel> PrepareNewAssetLocationViewModelAsync(int areaId)
        {
            var area = await _context.Areas
                .Where(a => a.areaId == areaId)
                .Select(a => new { a.areaId, a.AreaName })
                .FirstOrDefaultAsync();

            //if (area == null)
            //    throw new NotFoundException("Area not found.");

            return new AssetLocationViewModel
            {
                areaId = area.areaId,
                AreaName = area.AreaName,
                IsActive = true
            };
        }

        public async Task AddAssetLocationAsync(AssetLocation assetLocation)
        {
            try
            {
                await _context.AssetLocations.AddAsync(assetLocation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Add asset location failed", ex);
            }
        }

        public async Task UpdateAssetLocationAsync(AssetLocation assetLocation)
        {
            try
            {
                _context.AssetLocations.Update(assetLocation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Update asset location failed", ex);
            }
        }
    }
}
