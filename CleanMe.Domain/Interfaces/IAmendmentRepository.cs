using CleanMe.Domain.Entities;

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