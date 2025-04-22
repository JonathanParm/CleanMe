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
    public class StockCodeRepository : IStockCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public StockCodeRepository( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockCode>> GetAllStockCodesAsync()
        {
            return await _context.StockCodes
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<StockCode?> GetStockCodeByIdAsync(int stockCodeId)
        {
            return await _context.StockCodes.FindAsync(stockCodeId);
        }

        public async Task<StockCode?> GetStockCodeWithAssetTypesByIdAsync(int stockCodeId)
        {
            return await _context.StockCodes
                .Include(a => a.AssetTypes.Where(at => !at.IsDeleted))
                .FirstOrDefaultAsync(a => a.stockCodeId == stockCodeId);
        }

        public async Task AddStockCodeAsync(StockCode StockCode)
        {
            await _context.StockCodes.AddAsync(StockCode);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockCodeAsync(StockCode StockCode)
        {
            _context.StockCodes.Update(StockCode);
            await _context.SaveChangesAsync();
        }
    }
}
