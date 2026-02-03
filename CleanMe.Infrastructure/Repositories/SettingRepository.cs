using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMe.Infrastructure.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDbContext _context;

        public SettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Setting> GetSettingAsync()
        {
            return await _context.Settings
                .FirstOrDefaultAsync(c => c.settingId == 1);
        }

        public async Task UpdateSettingAsync(Setting Setting)
        {
            if (Setting.settingId == 0)
                throw new ArgumentException("Invalid settingId for update.");

            try
            {
                _context.Settings.Update(Setting);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Update setting failed", ex);
            }
        }
    }
}
