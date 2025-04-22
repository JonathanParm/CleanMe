using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IAssetTypeService
    {
        Task<IEnumerable<AssetTypeIndexViewModel>> GetAssetTypeIndexAsync(
            string? name, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<AssetTypeViewModel>> FindDuplicateAssetTypeAsync(string name, int? excludeAssetTypeId);
        Task<AssetTypeViewModel?> GetAssetTypeViewModelByIdAsync(int assetTypeId);
        Task<AssetTypeViewModel> PrepareNewAssetTypeViewModelAsync(int stockCodeId);
        Task<int> AddAssetTypeAsync(AssetTypeViewModel model, string addedById);
        Task UpdateAssetTypeAsync(AssetTypeViewModel model, string updatedById);
        Task<bool> SoftDeleteAssetTypeAsync(int assetTypeId, string updatedById);
    }
}