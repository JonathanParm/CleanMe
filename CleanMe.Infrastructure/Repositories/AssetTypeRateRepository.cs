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
    public class AssetTypeRateRepository : IAssetTypeRateRepository
    {
        private readonly ApplicationDbContext _context;

        public AssetTypeRateRepository( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssetTypeRate>> GetAllAssetTypeRatesAsync()
        {
            return await _context.AssetTypeRates
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<AssetTypeRate?> GetAssetTypeRateByIdAsync(int assetTypeRateId)
        {
            return await _context.AssetTypeRates
                .Include(a => a.AssetType)  // eager load
                .Include(a => a.CleanFrequency)
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.assetTypeRateId == assetTypeRateId);
        }

        public async Task<AssetTypeRate?> GetAssetTypeRateOnlyByIdAsync(int assetTypeRateId)
        {
            return await _context.AssetTypeRates
                .FirstOrDefaultAsync(a => a.assetTypeRateId == assetTypeRateId);
        }

        public async Task AddAssetTypeRateAsync(AssetTypeRate AssetTypeRate)
        {
            await _context.AssetTypeRates.AddAsync(AssetTypeRate);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssetTypeRateAsync(AssetTypeRate AssetTypeRate)
        {
            _context.AssetTypeRates.Update(AssetTypeRate);
            await _context.SaveChangesAsync();
        }
    }
}
