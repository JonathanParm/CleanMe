using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaViewModel>> FindDuplicateAreaAsync(string name, int code, int? excludeAreaId);

        Task<AreaViewModel?> GetAreaViewModelByIdAsync(int areaId);
        Task<AreaViewModel?> GetAreaViewModelWithAssetLocationsByIdAsync(int areaId);
        Task<AreaViewModel> PrepareNewAreaViewModelAsync(int regionId);
        Task<int> AddAreaAsync(AreaViewModel model, string addedById);
        Task UpdateAreaAsync(AreaViewModel model, string updatedById);
        Task<bool> SoftDeleteAreaAsync(int AreaId, string updatedById);

        // Retrieves a paginated & filtered Area list using Dapper
        Task<IEnumerable<AreaIndexViewModel>> GetAreaIndexAsync(
                string? regionName, string? name, int? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
    }
}