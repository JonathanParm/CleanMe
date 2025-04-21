using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IAssetTypeRateService
    {
        Task<IEnumerable<AssetTypeRateIndexViewModel>> GetAssetTypeRateIndexAsync(
                string? rateName, string? assetTypeName, string? cleanFreqName, string? clientName,
                decimal? rate, string? isDefault, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<AssetTypeRateViewModel>> FindDuplicateAssetTypeRateAsync(int assetTypeId, int cleanFrequencyId, int clientId, int? assetTypeRateId);

        Task<AssetTypeRateViewModel?> GetAssetTypeRateViewModelByIdAsync(int assetTypeRateId);
        Task<AssetTypeRateViewModel?> GetAssetTypeRateViewModelOnlyByIdAsync(int assetTypeRateId);
        //Task<AssetTypeRateViewModel> PrepareNewAssetTypeRateViewModelAsync(int regionId);
        
        Task<int> AddAssetTypeRateAsync(AssetTypeRateViewModel model, string addedById);
        Task UpdateAssetTypeRateAsync(AssetTypeRateViewModel model, string updatedById);
        Task<bool> SoftDeleteAssetTypeRateAsync(int assetTypeRateId, string updatedById);
    }
}