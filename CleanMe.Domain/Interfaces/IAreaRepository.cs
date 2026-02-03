using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAreasAsync();
        Task<Area?> GetAreaByIdAsync(int areaId);
        //Task<Area?> GetAreaWithAssetLocationsByIdAsync(int areaId);
        Task<int> GetAreaAssetLocationsCountAsync(int areaId);
        Task<List<AssetLocation>> GetAreaAssetLocationsPagedAsync(int areaId, int pageNumber, int pageSize);
        Task AddAreaAsync(Area Area);
        Task UpdateAreaAsync(Area Area);
    }
}