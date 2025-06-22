using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IItemCodeRepository
    {
        Task<IEnumerable<ItemCode>> GetAllItemCodesAsync();
        Task<ItemCode?> GetItemCodeByIdAsync(int itemCodeId);
        Task<ItemCode?> GetItemCodeWithItemCodeRatesByIdAsync(int itemCodeId);
        Task AddItemCodeAsync(ItemCode itemCode);
        Task UpdateItemCodeAsync(ItemCode itemCode);
    }
}