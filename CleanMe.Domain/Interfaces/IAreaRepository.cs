using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAreasAsync();
        Task<Area?> GetAreaByIdAsync(int areaId);
        Task<Area?> GetAreaWithAssetLocationsByIdAsync(int areaId);
        Task AddAreaAsync(Area Area);
        Task UpdateAreaAsync(Area Area);
    }
}