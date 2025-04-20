using CleanMe.Domain.Interfaces;
using CleanMe.Infrastructure.Data;

namespace CleanMe.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IAreaRepository AreaRepository { get; }
        public IAssetLocationRepository AssetLocationRepository { get; }
        public IAssetRepository AssetRepository { get; }
        public IAssetTypeRepository AssetTypeRepository { get; }
        public ICleanFrequencyRepository CleanFrequencyRepository { get; }
        public IClientContactRepository ClientContactRepository { get; }
        public IClientRepository ClientRepository { get; }
        public IDapperRepository DapperRepository { get; }
        public IRegionRepository RegionRepository { get; }
        public IStaffRepository StaffRepository { get; }

        public UnitOfWork(
                ApplicationDbContext context,
                IAreaRepository areaRepository,
                IAssetLocationRepository assetLocationRepository,
                IAssetRepository assetRepository,
                IAssetTypeRepository assetTypeRepository,
                ICleanFrequencyRepository cleanFrequencyRepository,
                IClientContactRepository clientContactRepository,
                IClientRepository clientRepository,
                IDapperRepository dapperRepository,
                IRegionRepository regionRepository,
                IStaffRepository staffRepository
            )
        {
            _context = context;
            AreaRepository = areaRepository;
            AssetLocationRepository = assetLocationRepository;
            AssetRepository = assetRepository;
            AssetTypeRepository = assetTypeRepository;
            CleanFrequencyRepository = cleanFrequencyRepository;
            ClientContactRepository = clientContactRepository;
            ClientRepository = clientRepository;
            DapperRepository = dapperRepository;
            RegionRepository = regionRepository;
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

