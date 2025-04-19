using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface IAssetTypeRepository
    {
        Task<IEnumerable<AssetTypeIndexViewModel>> GetAssetTypeIndexAsync(
                string? name, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<AssetType?> GetAssetTypeByIdAsync(int id);
        Task AddAssetTypeAsync(AssetType AssetType, string createdById);
        Task UpdateAssetTypeAsync(AssetType AssetType, string updatedById);
        Task SoftDeleteAssetTypeAsync(int id, string deletedById);
    }
}