using CleanMe.Application.Helpers.Paging;
using CleanMe.Application.ViewModels;

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
        Task<PagedResult<CleanFrequencyIndexViewModel>> GetPagedIndexAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortOrder,
            string? cleanFrequencyName,
            string? description,
            string? code,
            string? isActive
        );
    }
}