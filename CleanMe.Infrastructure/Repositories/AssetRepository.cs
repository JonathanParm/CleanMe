using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMe.Infrastructure.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly ApplicationDbContext _context;

        public AssetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            return await _context.Assets
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.AssetName)
                .ToListAsync();
        }

        public async Task<Asset?> GetAssetByIdAsync(int assetId)
        {
            return await _context.Assets.FindAsync(assetId);
        }

        public async Task AddAssetAsync(Asset Asset)
        {
            try
            {
                await _context.Assets.AddAsync(Asset);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Add asset failed", ex);
            }
        }

        public async Task UpdateAssetAsync(Asset Asset)
        {
            try
            {
                _context.Assets.Update(Asset);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Update asset failed", ex);
            }
        }
    }
}
