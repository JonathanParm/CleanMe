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
    public class ItemCodeRateRepository : IItemCodeRateRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemCodeRateRepository( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemCodeRate>> GetAllItemCodeRatesAsync()
        {
            return await _context.ItemCodeRates
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<ItemCodeRate?> GetItemCodeRateByIdAsync(int itemCodeRateId)
        {
            return await _context.ItemCodeRates
                .Include(a => a.ItemCode)  // eager load
                .Include(a => a.CleanFrequency)
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.itemCodeRateId == itemCodeRateId);
        }

        public async Task<ItemCodeRate?> GetItemCodeRateOnlyByIdAsync(int itemCodeRateId)
        {
            return await _context.ItemCodeRates
                .FirstOrDefaultAsync(a => a.itemCodeRateId == itemCodeRateId);
        }

        public async Task AddItemCodeRateAsync(ItemCodeRate ItemCodeRate)
        {
            await _context.ItemCodeRates.AddAsync(ItemCodeRate);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemCodeRateAsync(ItemCodeRate ItemCodeRate)
        {
            _context.ItemCodeRates.Update(ItemCodeRate);
            await _context.SaveChangesAsync();
        }
    }
}
