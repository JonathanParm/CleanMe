using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IStockCodeRepository
    {
        Task<IEnumerable<StockCode>> GetAllStockCodesAsync();
        Task<StockCode?> GetStockCodeByIdAsync(int stockCodeId);
        Task<StockCode?> GetStockCodeWithAssetTypesByIdAsync(int stockCodeId);
        Task AddStockCodeAsync(StockCode StockCode);
        Task UpdateStockCodeAsync(StockCode StockCode);
    }
}