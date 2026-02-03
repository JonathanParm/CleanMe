using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMe.Infrastructure.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public AreaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Area>> GetAllAreasAsync()
        {
            return await _context.Areas
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.AreaName)
                .ToListAsync();
        }

        public async Task<Area?> GetAreaByIdAsync(int areaId)
        {
            return await _context.Areas
                .AsNoTracking()
                .Include(a => a.Region)  // eager load Region
                .FirstOrDefaultAsync(a => a.areaId == areaId);
        }

        //public async Task<Area?> GetAreaWithAssetLocationsByIdAsync(int areaId)
        //{
        //    return await _context.Areas
        //        .Include(a => a.Region) // parent region
        //        .Include(a => a.AssetLocations.Where(al => !al.IsDeleted))
        //        .FirstOrDefaultAsync(a => a.areaId == areaId);
        //}
        public async Task<int> GetAreaAssetLocationsCountAsync(int areaId)
        {
            return await _context.AssetLocations
                .AsNoTracking()
                .Where(a => a.areaId == areaId && !a.IsDeleted)
                .CountAsync();
        }
        public async Task<List<AssetLocation>> GetAreaAssetLocationsPagedAsync(
            int areaId,
            int pageNumber,
            int pageSize)
        {
            return await _context.AssetLocations
                .AsNoTracking()
                .Where(a => a.areaId == areaId && !a.IsDeleted)
                .OrderBy(a => a.Description)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAreaAsync(Area Area)
        {
            try
            {
                await _context.Areas.AddAsync(Area);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Add area failed", ex);
            }
        }

        public async Task UpdateAreaAsync(Area Area)
        {
            try
            {
                _context.Areas.Update(Area);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Update area failed", ex);
            }
        }
    }
}
