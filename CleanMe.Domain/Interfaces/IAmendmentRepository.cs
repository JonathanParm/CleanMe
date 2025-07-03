using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface IAmendmentRepository
    {
        Task<IEnumerable<Amendment>> GetAllAmendmentsAsync();
        Task<Amendment?> GetAmendmentByIdAsync(int amendmentId);
        Task<Amendment?> GetAmendmentOnlyByIdAsync(int amendmentId);
        Task AddAmendmentAsync(Amendment amendment);
        Task UpdateAmendmentAsync(Amendment amendment);
    }
}