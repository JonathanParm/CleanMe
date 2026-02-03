using CleanMe.Application.Helpers.Paging;
using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionIndexViewModel>> GetRegionIndexAsync(
            string? name, string? code, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<RegionViewModel>> FindDuplicateRegionAsync(string name, string reportCode, int? excludeRegionId);
        Task<RegionViewModel?> GetRegionViewModelByIdAsync(int regionId);
        Task<RegionWithAreasViewModel?> GetRegionWithAreasViewModelByIdAsync(int regionId, int pageNumber, int pageSize);
        Task<int> AddRegionAsync(RegionViewModel model, string addedById);
        Task UpdateRegionAsync(RegionViewModel model, string updatedById);
        Task<bool> SoftDeleteRegionAsync(int regionId, string updatedById);
        Task<PagedResult<RegionIndexViewModel>> GetPagedIndexAsync(
                    int pageNumber,
                    int pageSize,
                    string sortColumn,
                    string sortOrder,
                    string? regionName,
                    string? reportCode,
                    string? isActive
                );
    }
}