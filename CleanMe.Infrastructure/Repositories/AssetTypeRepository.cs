using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

        public async Task<IEnumerable<AssetType>> GetAllAssetTypesAsync()
        {
            return await _context.AssetTypes
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<AssetType?> GetAssetTypeByIdAsync(int assetTypeId)
        {
            return await _context.AssetTypes
                .Include(a => a.StockCode)  // eager load
                .FirstOrDefaultAsync(a => a.assetTypeId == assetTypeId);
        }

        public async Task AddAssetTypeAsync(AssetType AssetType)
        {
            await _context.AssetTypes.AddAsync(AssetType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssetTypeAsync(AssetType AssetType)
        {
            _context.AssetTypes.Update(AssetType);
            await _context.SaveChangesAsync();
        }
    }
}
