using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface ICleanFrequencyService
    {
        Task<IEnumerable<CleanFrequencyIndexViewModel>> GetCleanFrequencyIndexAsync(
            string? name, string? description, string? code, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<CleanFrequencyViewModel>> FindDuplicateCleanFrequencyAsync(string name, string code, int? excludeCleanFrequencyId);

        Task<CleanFrequencyViewModel?> GetCleanFrequencyViewModelByIdAsync(int cleanFrequencyId);
        Task<int> AddCleanFrequencyAsync(CleanFrequencyViewModel model, string addedById);
        Task UpdateCleanFrequencyAsync(CleanFrequencyViewModel model, string updatedById);
        Task<bool> SoftDeleteCleanFrequencyAsync(int cleanFrequencyId, string updatedById);
    }
}