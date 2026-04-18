using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanMe.Infrastructure.Repositories
{
    public class AmendmentRepository : IAmendmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AmendmentRepository> _logger;

        public AmendmentRepository(ApplicationDbContext context, ILogger<AmendmentRepository> logger)
        {
            _context = context;
            _logger = logger;
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
            try
            {
                // use .Include(...) on navigation properties — not scalar foreign keys
                return await _context.Amendments
                    .Include(a => a.Client)
                    .Include(a => a.Area)
                    .Include(a => a.AssetLocation)
                    .Include(a => a.ItemCode)
                    .Include(a => a.Asset)
                    .Include(a => a.CleanFrequency)
                    .Include(a => a.Staff)
                    .Include(a => a.AmendmentType)
                    .FirstOrDefaultAsync(a => a.amendmentId == amendmentId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while processing GetAmendmentByIdAsync.", ex);
            }
        }

        public async Task<Amendment?> GetAmendmentOnlyByIdAsync(int amendmentId)
        {
            return await _context.Amendments.FindAsync(amendmentId);
        }

        public async Task AddAmendmentAsync(Amendment amendment)
        {
            try
            {
                await _context.Amendments.AddAsync(amendment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "AddAmendmentAsync failed for Amendment {@Amendment}", amendment);
                throw;
            }
        }

        public async Task UpdateAmendmentAsync(Amendment amendment)
        {
            try
            {
                _context.Amendments.Update(amendment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "UpdateAmendmentAsync failed for Amendment {@Amendment}", amendment);
                throw;
            }
        }
    }
}
