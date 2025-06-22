namespace CleanMe.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAmendmentRepository AmendmentRepository { get; }
        IAmendmentTypeRepository AmendmentTypeRepository { get; }
        IAreaRepository AreaRepository { get; }
        IAssetLocationRepository AssetLocationRepository { get; }
        IAssetRepository AssetRepository { get; }
        ICleanFrequencyRepository CleanFrequencyRepository { get; }
        IClientContactRepository ClientContactRepository { get; }
        IClientRepository ClientRepository { get; }
        ICompanyInfoRepository CompanyInfoRepository { get; }
        IItemCodeRateRepository ItemCodeRateRepository { get; }
        IItemCodeRepository ItemCodeRepository { get; }
        IRegionRepository RegionRepository { get; }
        ISettingRepository SettingRepository { get; }
        IStaffRepository StaffRepository { get; }

        IDapperRepository DapperRepository { get; }

        Task<int> CommitAsync();
    }
}