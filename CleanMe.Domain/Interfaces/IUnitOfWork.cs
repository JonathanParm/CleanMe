namespace CleanMe.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAreaRepository AreaRepository { get; }
        IAssetLocationRepository AssetLocationRepository { get; }
        IAssetRepository AssetRepository { get; }
        IAssetTypeRepository AssetTypeRepository { get; }
        ICleanFrequencyRepository CleanFrequencyRepository { get; }
        IClientContactRepository ClientContactRepository { get; }
        IClientRepository ClientRepository { get; }
        IRegionRepository RegionRepository { get; }
        IStaffRepository StaffRepository { get; }

        IDapperRepository DapperRepository { get; }

        Task<int> CommitAsync();
    }
}