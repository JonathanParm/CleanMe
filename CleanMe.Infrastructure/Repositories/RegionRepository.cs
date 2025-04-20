using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
                .OrderBy(c => c.Name)
                .ToListAsync();
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

        public async Task AddRegionAsync(Region region)
        {
            await _context.Regions.AddAsync(region);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRegionAsync(Region region)
        {
            _context.Regions.Update(region);
            await _context.SaveChangesAsync();
        }
    }
}
