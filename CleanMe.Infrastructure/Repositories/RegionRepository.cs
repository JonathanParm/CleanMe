using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMe.Infrastructure.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly ApplicationDbContext _context;

        public RegionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Region>> GetAllRegionsAsync()
        {
            return await _context.Regions
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.RegionName)
                .ToListAsync();
        }

        //public async Task<Region?> GetRegionByIdAsync(int regionId)
        //{
        //    return await _context.Regions.FindAsync(regionId);
        //}

        //public async Task<Region?> GetRegionWithAreasByIdAsync(int regionId, int pageNumber, int pageSize)
        //{
        //    return await _context.Regions
        //        .Include(r => r.Areas.Where(a => !a.IsDeleted))
        //        .FirstOrDefaultAsync(r => r.regionId == regionId);
        //}
        public async Task<Region?> GetRegionByIdAsync(int regionId)
        {
            return await _context.Regions
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.regionId == regionId);
        }

        public async Task<int> GetRegionAreaCountAsync(int regionId)
        {
            return await _context.Areas
                .AsNoTracking()
                .Where(a => a.regionId == regionId && !a.IsDeleted)
                .CountAsync();
        }

        public async Task<List<Area>> GetRegionAreasPagedAsync(
            int regionId,
            int pageNumber,
            int pageSize)
        {
            return await _context.Areas
                .AsNoTracking()
                .Where(a => a.regionId == regionId && !a.IsDeleted)
                .OrderBy(a => a.AreaName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task AddRegionAsync(Region region)
        {
            try
            {
                await _context.Regions.AddAsync(region);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Add region failed", ex);
            }
        }

        public async Task UpdateRegionAsync(Region region)
        {
            try
            {
                _context.Regions.Update(region);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Update region failed", ex);
            }
        }
    }
}
