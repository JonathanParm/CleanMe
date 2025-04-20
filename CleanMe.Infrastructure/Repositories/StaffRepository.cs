// --- Namespace: CleanMe.Infrastructure.Repositories ---
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;

namespace CleanMe.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Staff?> GetStaffByIdAsync(int staffId)
        {
            return await _context.Staff.FindAsync(staffId);
        }

        public async Task AddStaffAsync(Staff Staff)
        {
            await _context.Staff.AddAsync(Staff);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStaffAsync(Staff Staff)
        {
            _context.Staff.Update(Staff);
            await _context.SaveChangesAsync();
        }
    }
}
