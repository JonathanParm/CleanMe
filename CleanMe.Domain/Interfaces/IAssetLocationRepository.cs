using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IAssetLocationRepository
    {
        Task<IEnumerable<AssetLocation>> GetAllAssetLocationsAsync();
        Task<AssetLocation?> GetAssetLocationByIdAsync(int assetLocationId);
        Task AddAssetLocationAsync(AssetLocation AssetLocation);
        Task UpdateAssetLocationAsync(AssetLocation AssetLocation);
    }
}