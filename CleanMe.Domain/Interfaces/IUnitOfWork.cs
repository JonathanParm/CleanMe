namespace CleanMe.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAreaRepository AreaRepository { get; }
        IAssetLocationRepository AssetLocationRepository { get; }
        IAssetRepository AssetRepository { get; }
        IAssetTypeRepository AssetTypeRepository { get; }
        IAssetTypeRateRepository AssetTypeRateRepository { get; }
        ICleanFrequencyRepository CleanFrequencyRepository { get; }
        IClientContactRepository ClientContactRepository { get; }
        IClientRepository ClientRepository { get; }
        IRegionRepository RegionRepository { get; }
        IStaffRepository StaffRepository { get; }
        IStockCodeRepository StockCodeRepository { get; }

        IDapperRepository DapperRepository { get; }

        Task<int> CommitAsync();
    }
}