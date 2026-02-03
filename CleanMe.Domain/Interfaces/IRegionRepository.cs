using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> GetAllRegionsAsync();
        Task<Region?> GetRegionByIdAsync(int regionId);
        //Task<Region?> GetRegionWithAreasByIdAsync(int regionId, int pageNumber, int pageSize);
        Task<int> GetRegionAreaCountAsync(int regionId);
        Task<List<Area>> GetRegionAreasPagedAsync(int regionId, int pageNumber, int pageSize);
        Task AddRegionAsync(Region region);
        Task UpdateRegionAsync(Region region);
    }
}