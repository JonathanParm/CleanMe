// --- Namespace: CleanMe.Infrastructure.Repositories ---
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMe.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Staff>> GetAllStaffAsync()
        {
            return await _context.Staff
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.FamilyName)
                .ToListAsync();
        }

        public async Task<Staff?> GetStaffByIdAsync(int staffId)
        {
            return await _context.Staff.FindAsync(staffId);
        }

        public async Task AddStaffAsync(Staff Staff)
        {
            try
            {
                await _context.Staff.AddAsync(Staff);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Add staff failed", ex);
            }
        }

        public async Task UpdateStaffAsync(Staff Staff)
        {
            try
            {
                _context.Staff.Update(Staff);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Update staff failed", ex);
            }
        }
    }
}
