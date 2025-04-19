using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionViewModel>> FindDuplicateRegionAsync(string name, string reportCode, int? excludeRegionId);

        Task<RegionViewModel?> GetRegionByIdAsync(int regionId);
        Task<RegionViewModel?> GetRegionWithAreasByIdAsync(int regionId);
        // Creates a new region using EF Core
        Task<int> AddRegionAsync(RegionViewModel model, string addedById);
        // Updates an existing region using EF Core
        Task UpdateRegionAsync(RegionViewModel model, string updatedById);
        Task<bool> SoftDeleteRegionAsync(int regionId, string updatedById);

        // Retrieves a paginated & filtered region list using Dapper
        Task<IEnumerable<RegionIndexViewModel>> GetRegionIndexAsync(
            string? name,
            string? code,
            string? isActive,
            string sortColumn,
            string sortOrder,
            int pageNumber,
            int pageSize);
    }
}