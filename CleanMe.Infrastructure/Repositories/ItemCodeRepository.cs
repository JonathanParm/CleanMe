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
    public class ItemCodeRepository : IItemCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemCode>> GetAllItemCodesAsync()
        {
            return await _context.ItemCodes
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Code)
                .ToListAsync();
        }

        public async Task<ItemCode?> GetItemCodeByIdAsync(int itemCodeId)
        {
            return await _context.ItemCodes.FindAsync(itemCodeId);
        }

        public async Task<ItemCode?> GetItemCodeWithItemCodeRatesByIdAsync(int itemCodeId)
        {
            return await _context.ItemCodes
                //.Include(a => a.ItemCodeRates.Where(icr => !icr.IsDeleted))
                .FirstOrDefaultAsync(a => a.itemCodeId == itemCodeId);
        }

        public async Task AddItemCodeAsync(ItemCode itemCode)
        {
            try
            {
                await _context.ItemCodes.AddAsync(itemCode);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Add item code failed", ex);
            }
        }

        public async Task UpdateItemCodeAsync(ItemCode itemCode)
        {
            try
            {
                _context.ItemCodes.Update(itemCode);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Update item code failed", ex);
            }
        }
    }
}
