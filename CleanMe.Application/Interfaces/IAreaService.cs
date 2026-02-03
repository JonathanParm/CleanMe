using CleanMe.Application.Helpers.Paging;
using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaViewModel>> FindDuplicateAreaAsync(string name, int code, int? excludeAreaId);

        Task<AreaViewModel?> GetAreaViewModelByIdAsync(int areaId);
        //Task<AreaViewModel?> GetAreaViewModelWithAssetLocationsByIdAsync(int areaId);
        Task<AreaWithAssetLocationsViewModel?> GetAreaWithAssetLocationsViewModelByIdAsync(int areaId, int pageNumber, int pageSize);

        Task<AreaWithAssetLocationsViewModel> PrepareNewAreaViewModelAsync(int regionId, int pageNumber, int pageSize);
        Task<int> AddAreaAsync(AreaViewModel model, string addedById);
        Task UpdateAreaAsync(AreaViewModel model, string updatedById);
        Task<bool> SoftDeleteAreaAsync(int AreaId, string updatedById);

        // Retrieves a paginated & filtered Area list using Dapper
        Task<IEnumerable<AreaIndexViewModel>> GetAreaIndexAsync(
                string? regionName, string? name, int? reportCode, string? cleanerName, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        //object GetAreaByIdAsync(int v);

        Task<PagedResult<AreaIndexViewModel>> GetPagedIndexAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortOrder,
            string? regionName,
            string? areaName,
            int? reportCode,
            string? cleanerName,
            string? isActive
        );
    }
}