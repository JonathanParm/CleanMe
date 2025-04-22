using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IStockCodeService
    {
        // Retrieves a paginated & filtered StockCode list using Dapper
        Task<IEnumerable<StockCodeIndexViewModel>> GetStockCodeIndexAsync(
                string? name, string? description, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);

        Task<IEnumerable<StockCodeViewModel>> FindDuplicateStockCodeAsync(string name, int? excludestockCodeId);

        Task<StockCodeViewModel?> GetStockCodeViewModelByIdAsync(int stockCodeId);
        Task<StockCodeViewModel?> GetStockCodeViewModelWithAssetTypesByIdAsync(int stockCodeId);
        Task<int> AddStockCodeAsync(StockCodeViewModel model, string addedById);
        Task UpdateStockCodeAsync(StockCodeViewModel model, string updatedById);
        Task<bool> SoftDeleteStockCodeAsync(int stockCodeId, string updatedById);
    }
}