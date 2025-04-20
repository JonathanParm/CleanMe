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
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Asset?> GetAssetByIdAsync(int assetId)
        {
            return await _context.Assets.FindAsync(assetId);
        }

        public async Task AddAssetAsync(Asset Asset)
        {
            await _context.Assets.AddAsync(Asset);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssetAsync(Asset Asset)
        {
            _context.Assets.Update(Asset);
            await _context.SaveChangesAsync();
        }
    }
}
