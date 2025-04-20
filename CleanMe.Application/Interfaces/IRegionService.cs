using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionIndexViewModel>> GetRegionIndexAsync(
            string? name, string? code, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<IEnumerable<RegionViewModel>> FindDuplicateRegionAsync(string name, string reportCode, int? excludeRegionId);
        Task<RegionViewModel?> GetRegionViewModelByIdAsync(int regionId);
        Task<RegionViewModel?> GetRegionViewModelWithAreasByIdAsync(int regionId);
        Task<int> AddRegionAsync(RegionViewModel model, string addedById);
        Task UpdateRegionAsync(RegionViewModel model, string updatedById);
        Task<bool> SoftDeleteRegionAsync(int regionId, string updatedById);

        // Retrieves a paginated & filtered region list using Dapper
    }
}