using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaViewModel>> FindDuplicateAreaAsync(string name, int code, int? excludeAreaId);

        Task<AreaViewModel?> GetAreaByIdAsync(int areaId);
        Task<AreaViewModel?> GetAreaViewModelByIdAsync(int areaId);
        Task<AreaViewModel?> GetAreaViewModelWithAssetLocationsByIdAsync(int areaId);
        Task<AreaViewModel> PrepareNewAreaViewModelAsync(int regionId);
        // Creates a new Area using EF Core
        Task<int> AddAreaAsync(AreaViewModel model, string addedById);
        // Updates an existing Area using EF Core
        Task UpdateAreaAsync(AreaViewModel model, string updatedById);
        Task<bool> SoftDeleteAreaAsync(int AreaId, string updatedById);

        // Retrieves a paginated & filtered Area list using Dapper
        Task<IEnumerable<AreaIndexViewModel>> GetAreaIndexAsync(
                string? regionName, string? name, int? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
    }
}