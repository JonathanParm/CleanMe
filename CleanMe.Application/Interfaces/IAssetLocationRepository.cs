using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface IAssetLocationRepository
    {
        Task<IEnumerable<AssetLocationIndexViewModel>> GetAssetLocationIndexAsync(
            string? areaName, string? description, string? townSuburb, string? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        //Task<AssetLocationViewModel?> GetAssetLocationByIdAsync(int assetLocationId);
        Task<AssetLocationViewModel?> GetAssetLocationViewModelByIdAsync(int assetLocationId);
        Task<AssetLocationViewModel?> PrepareNewAssetLocationViewModelAsync(int areaId);
        Task AddAssetLocationAsync(AssetLocation AssetLocation, string createdById);
        Task UpdateAssetLocationAsync(AssetLocation AssetLocation, string updatedById);
        Task SoftDeleteAssetLocationAsync(int assetLocationId, string deletedById);
    }
}