using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface ICleanFrequencyRepository
    {
        Task<IEnumerable<CleanFrequency>> GetAllCleanFrequenciesAsync();
        Task<CleanFrequency?> GetCleanFrequencyByIdAsync(int cleanFrequencyId);
        Task AddCleanFrequencyAsync(CleanFrequency cleanFrequency);
        Task UpdateCleanFrequencyAsync(CleanFrequency cleanFrequency);
    }
}