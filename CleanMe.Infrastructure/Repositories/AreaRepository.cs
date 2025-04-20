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
        private readonly ApplicationDbContext _context;

        public AreaRepository( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Area>> GetAllAreasAsync()
        {
            return await _context.Areas
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Area?> GetAreaByIdAsync(int areaId)
        {
            return await _context.Areas
                .Include(a => a.Region)  // eager load Region
                .FirstOrDefaultAsync(a => a.areaId == areaId);
        }

        public async Task<Area?> GetAreaWithAssetLocationsByIdAsync(int areaId)
        {
            return await _context.Areas
                .Include(a => a.Region) // parent region
                .Include(a => a.AssetLocations.Where(al => !al.IsDeleted))
                .FirstOrDefaultAsync(a => a.areaId == areaId);
        }

        public async Task AddAreaAsync(Area Area)
        {
            await _context.Areas.AddAsync(Area);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAreaAsync(Area Area)
        {
            _context.Areas.Update(Area);
            await _context.SaveChangesAsync();
        }
    }
}
