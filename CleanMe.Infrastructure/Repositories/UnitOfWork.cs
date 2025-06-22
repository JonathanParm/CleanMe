using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;

namespace CleanMe.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IAmendmentRepository AmendmentRepository { get; }
        public IAmendmentTypeRepository AmendmentTypeRepository { get; }
        public IAreaRepository AreaRepository { get; }
        public IAssetLocationRepository AssetLocationRepository { get; }
        public IAssetRepository AssetRepository { get; }
        public ICleanFrequencyRepository CleanFrequencyRepository { get; }
        public IClientContactRepository ClientContactRepository { get; }
        public IClientRepository ClientRepository { get; }
        public ICompanyInfoRepository CompanyInfoRepository { get; }
        public IDapperRepository DapperRepository { get; }
        public IItemCodeRateRepository ItemCodeRateRepository { get; }
        public IItemCodeRepository ItemCodeRepository { get; }
        public IRegionRepository RegionRepository { get; }
        public ISettingRepository SettingRepository { get; }
        public IStaffRepository StaffRepository { get; }

        public UnitOfWork(
                ApplicationDbContext context,
                IAmendmentRepository amendmentRepository,
                IAmendmentTypeRepository amendmentTypeRepository,
                IAreaRepository areaRepository,
                IAssetLocationRepository assetLocationRepository,
                IAssetRepository assetRepository,
                ICleanFrequencyRepository cleanFrequencyRepository,
                IClientContactRepository clientContactRepository,
                IClientRepository clientRepository,
                ICompanyInfoRepository companyInfoRepository,
                IDapperRepository dapperRepository,
                IItemCodeRateRepository itemCodeRateRepository,
                IItemCodeRepository itemCodeRepository,
                IRegionRepository regionRepository,
                ISettingRepository settingRepository,
                IStaffRepository staffRepository
            )
        {
            _context = context;
            AmendmentRepository = amendmentRepository;
            AmendmentTypeRepository = amendmentTypeRepository;
            AreaRepository = areaRepository;
            AssetLocationRepository = assetLocationRepository;
            AssetRepository = assetRepository;
            CleanFrequencyRepository = cleanFrequencyRepository;
            ClientContactRepository = clientContactRepository;
            ClientRepository = clientRepository;
            CompanyInfoRepository = companyInfoRepository;
            DapperRepository = dapperRepository;
            ItemCodeRateRepository = itemCodeRateRepository;
            ItemCodeRepository = itemCodeRepository;
            RegionRepository = regionRepository;
            SettingRepository = settingRepository;
            StaffRepository = staffRepository;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

