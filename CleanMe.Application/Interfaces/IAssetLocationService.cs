using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IAssetLocationService
    {
        // Retrieves a paginated & filtered AssetLocation list using Dapper
        Task<IEnumerable<AssetLocationIndexViewModel>> GetAssetLocationIndexAsync(
            string? areaName, string? description, string? townSuburb, string? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);

        Task<IEnumerable<AssetLocationViewModel>> FindDuplicateAssetLocationAsync(string description, string reportCode, int? excludeAssetLocationId);
        Task<AssetLocationViewModel?> GetAssetLocationViewModelByIdAsync(int assetLocationId);
        Task<AssetLocationViewModel> PrepareNewAssetLocationViewModelAsync(int areaId);
        // Creates a new AssetLocation member using EF Core
        Task<int> AddAssetLocationAsync(AssetLocationViewModel model, string addedById);
        // Updates an existing AssetLocation member using EF Core
        Task UpdateAssetLocationAsync(AssetLocationViewModel model, string updatedById);
        Task UpdateAssetLocationApplicationUserId(int assetLocationId, string applicationUserId);
        Task<bool> SoftDeleteAssetLocationAsync(int assetLocationId, string updatedById);

    }
}