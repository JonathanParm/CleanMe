using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IItemCodeRateService
    {
        Task<IEnumerable<ItemCodeRateIndexViewModel>> GetItemCodeRateIndexAsync(
                string? rateName, string? itemCodeName, string? cleanFreqName, string? clientName,
                decimal? rate, string? isDefault, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<ItemCodeRateViewModel>> FindDuplicateItemCodeRateAsync(int itemCodeId, int cleanFrequencyId, int clientId, int? itemCodeRateId);

        Task<ItemCodeRateViewModel?> GetItemCodeRateViewModelByIdAsync(int itemCodeRateId);
        Task<ItemCodeRateViewModel?> GetItemCodeRateViewModelOnlyByIdAsync(int itemCodeRateId);
        //Task<ItemCodeRateViewModel> PrepareNewItemCodeRateViewModelAsync(int regionId);
        
        Task<int> AddItemCodeRateAsync(ItemCodeRateViewModel model, string addedById);
        Task UpdateItemCodeRateAsync(ItemCodeRateViewModel model, string updatedById);
        Task<bool> SoftDeleteItemCodeRateAsync(int itemCodeRateId, string updatedById);
    }
}