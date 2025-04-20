using CleanMe.Application.DTOs;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Common;
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _context.AssetLocations
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Description)
                .ToListAsync();
        }

        public async Task<AssetLocation?> GetAssetLocationByIdAsync(int assetLocationId)
        {
            return await _context.AssetLocations
                .FirstOrDefaultAsync(al => al.assetLocationId == assetLocationId);
        }

        public async Task<AssetLocationViewModel> PrepareNewAssetLocationViewModelAsync(int areaId)
        {
            var area = await _context.Areas
                .Where(a => a.areaId == areaId)
                .Select(a => new { a.areaId, a.Name })
                .FirstOrDefaultAsync();

            //if (area == null)
            //    throw new NotFoundException("Area not found.");

            return new AssetLocationViewModel
            {
                areaId = area.areaId,
                AreaName = area.Name,
                IsActive = true
            };
        }

        public async Task AddAssetLocationAsync(AssetLocation assetLocation)
        {
            await _context.AssetLocations.AddAsync(assetLocation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssetLocationAsync(AssetLocation assetLocation)
        {
            _context.AssetLocations.Update(assetLocation);
            await _context.SaveChangesAsync();
        }
    }
}
