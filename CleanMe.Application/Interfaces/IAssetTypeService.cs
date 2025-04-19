using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IAssetTypeService
    {
        Task<IEnumerable<AssetTypeViewModel>> FindDuplicateAssetTypeAsync(string name, int? excludeAssetTypeId);

        Task<AssetTypeViewModel?> GetAssetTypeByIdAsync(int assetTypeId);
        // Creates a new AssetType using EF Core
        Task<int> AddAssetTypeAsync(AssetTypeViewModel model, string addedById);
        // Updates an existing AssetType using EF Core
        Task UpdateAssetTypeAsync(AssetTypeViewModel model, string updatedById);
        Task<bool> SoftDeleteAssetTypeAsync(int assetTypeId, string updatedById);

        // Retrieves a paginated & filtered AssetType list using Dapper
        Task<IEnumerable<AssetTypeIndexViewModel>> GetAssetTypeIndexAsync(
            string? name,
            string? isActive,
            string sortColumn,
            string sortOrder,
            int pageNumber,
            int pageSize);
    }
}