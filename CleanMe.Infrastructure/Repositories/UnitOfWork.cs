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
        public IAssetTypeRepository AssetTypeRepository { get; }
        public IAssetTypeRateRepository AssetTypeRateRepository { get; }
        public ICleanFrequencyRepository CleanFrequencyRepository { get; }
        public IClientContactRepository ClientContactRepository { get; }
        public IClientRepository ClientRepository { get; }
        public IDapperRepository DapperRepository { get; }
        public IRegionRepository RegionRepository { get; }
        public IStaffRepository StaffRepository { get; }
        public IStockCodeRepository StockCodeRepository { get; }

        public UnitOfWork(
                ApplicationDbContext context,
                IAmendmentRepository amendmentRepository,
                IAmendmentTypeRepository amendmentTypeRepository,
                IAreaRepository areaRepository,
                IAssetLocationRepository assetLocationRepository,
                IAssetRepository assetRepository,
                IAssetTypeRepository assetTypeRepository,
                IAssetTypeRateRepository assetTypeRateRepository,
                ICleanFrequencyRepository cleanFrequencyRepository,
                IClientContactRepository clientContactRepository,
                IClientRepository clientRepository,
                IDapperRepository dapperRepository,
                IRegionRepository regionRepository,
                IStaffRepository staffRepository,
                IStockCodeRepository stockCodeRepository
            )
        {
            _context = context;
            AmendmentRepository = amendmentRepository;
            AmendmentTypeRepository = amendmentTypeRepository;
            AreaRepository = areaRepository;
            AssetLocationRepository = assetLocationRepository;
            AssetRepository = assetRepository;
            AssetTypeRepository = assetTypeRepository;
            AssetTypeRateRepository = assetTypeRateRepository;
            CleanFrequencyRepository = cleanFrequencyRepository;
            ClientContactRepository = clientContactRepository;
            ClientRepository = clientRepository;
            DapperRepository = dapperRepository;
            RegionRepository = regionRepository;
            StaffRepository = staffRepository;
            StockCodeRepository = stockCodeRepository;
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

