using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.DTOs;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetIndexViewModel>> GetAssetIndexAsync(
            string? assetName, string? regionName, string? mdReference, string? clientName, string? clientReference,
            string? assetLocation, string? assetType,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<AssetViewModel>> FindDuplicateAssetAsync(string name, int? excludeassetId);
        Task<AssetViewModel?> GetAssetViewModelByIdAsync(int assetId);
        Task<AssetHierarchyDto?> GetHierarchyIdsByAssetIdAsync(int assetId);
        Task<int> AddAssetAsync(AssetViewModel model, string addedById);
        Task UpdateAssetAsync(AssetViewModel model, string updatedById);
        Task<bool> SoftDeleteAssetAsync(int assetId, string updatedById);
    }
}