using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMe.Infrastructure.Repositories
{
    public class AmendmentRepository : IAmendmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AmendmentRepository( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Amendment>> GetAllAmendmentsAsync()
        {
            return await _context.Amendments
                .Where(c => !c.IsDeleted && !c.InvoicedOn.HasValue)
                .OrderBy(c => c.StartOn)
                .ThenBy(c => c.FinishOn)
                .ToListAsync();
        }

        public async Task<Amendment?> GetAmendmentByIdAsync(int amendmentId)
        {
            return await _context.Amendments
                .Include(a => a.clientId)  // eager load
                .Include(a => a.areaId)  // eager load
                .Include(a => a.assetLocationId)  // eager load
                .Include(a => a.assetId)  // eager load
                .Include(a => a.cleanFrequencyId)  // eager load
                .Include(a => a.staffId)  // eager load
                .FirstOrDefaultAsync(a => a.amendmentId == amendmentId);
        }

        public async Task<Amendment?> GetAmendmentOnlyByIdAsync(int amendmentId)
        {
            return await _context.Amendments.FindAsync(amendmentId);
        }

        public async Task AddAmendmentAsync(Amendment Amendment)
        {
            await _context.Amendments.AddAsync(Amendment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAmendmentAsync(Amendment Amendment)
        {
            _context.Amendments.Update(Amendment);
            await _context.SaveChangesAsync();
        }
    }
}
