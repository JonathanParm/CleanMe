using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Asset>> GetAllAssetsAsync();
        Task<Asset?> GetAssetByIdAsync(int assetId);
        Task AddAssetAsync(Asset Asset);
        Task UpdateAssetAsync(Asset Asset);
    }
}