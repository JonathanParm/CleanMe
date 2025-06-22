using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IItemCodeService
    {
        // Retrieves a paginated & filtered ItemCode list using Dapper
        Task<IEnumerable<ItemCodeIndexViewModel>> GetItemCodeIndexAsync(
                string? code, string? itemName,
                string? purchasesUnitRate, string? purchasesXeroAccount, string? salesUnitRate, string? salesXeroAccount,
                string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);

        Task<IEnumerable<ItemCodeViewModel>> FindDuplicateItemCodeAsync(string code, string itemName, int? excludeitemCodeId);

        Task<ItemCodeViewModel?> GetItemCodeViewModelByIdAsync(int itemCodeId);
        Task<ItemCodeViewModel?> GetItemCodeViewModelWithItemCodeRatesByIdAsync(int itemCodeId);
        Task<int> AddItemCodeAsync(ItemCodeViewModel model, string addedById);
        Task UpdateItemCodeAsync(ItemCodeViewModel model, string updatedById);
        Task<bool> SoftDeleteItemCodeAsync(int itemCodeId, string updatedById);
    }
}