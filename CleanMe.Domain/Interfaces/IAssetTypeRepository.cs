using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IAssetTypeRepository
    {
        Task<IEnumerable<AssetType>> GetAllAssetTypesAsync();
        Task<AssetType?> GetAssetTypeByIdAsync(int assetTypeId);
        Task AddAssetTypeAsync(AssetType AssetType);
        Task UpdateAssetTypeAsync(AssetType AssetType);
    }
}