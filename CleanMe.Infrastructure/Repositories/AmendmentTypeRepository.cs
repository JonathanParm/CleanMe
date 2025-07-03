using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanMe.Infrastructure.Repositories
{
    public class AmendmentTypeRepository : IAmendmentTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public AmendmentTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AmendmentType>> GetAllAmendmentTypesAsync()
        {
            return await _context.AmendmentTypes
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<AmendmentType?> GetAmendmentTypeByIdAsync(int amendmentTypeId)
        {
            return await _context.AmendmentTypes.FindAsync(amendmentTypeId);
        }

        //public async Task<AmendmentType?> GetAmendmentTypeOnlyByIdAsync(int amendmentTypeId)
        //{
        //    return await _context.AmendmentTypes
        //        .Include(a => a.Amendments.Where(at => !at.IsDeleted))
        //        .FirstOrDefaultAsync(a => a.amendmentTypeId == amendmentTypeId);
        //}

        public async Task AddAmendmentTypeAsync(AmendmentType AmendmentType)
        {
            try
            {
                await _context.AmendmentTypes.AddAsync(AmendmentType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Add amendment type failed", ex);
            }
        }

        public async Task UpdateAmendmentTypeAsync(AmendmentType AmendmentType)
        {
            try
            {
                _context.AmendmentTypes.Update(AmendmentType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log or debug ex.InnerException
                throw new Exception("Update amendment type failed", ex);
            }
        }
    }
}
