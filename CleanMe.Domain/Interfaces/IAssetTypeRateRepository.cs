using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface IAssetTypeRateRepository
    {
        Task<IEnumerable<AssetTypeRate>> GetAllAssetTypeRatesAsync();
        Task<AssetTypeRate?> GetAssetTypeRateByIdAsync(int assetTypeRateId);
        Task<AssetTypeRate?> GetAssetTypeRateOnlyByIdAsync(int assetTypeRateId);
        Task AddAssetTypeRateAsync(AssetTypeRate AssetTypeRate);
        Task UpdateAssetTypeRateAsync(AssetTypeRate AssetTypeRate);
    }
}