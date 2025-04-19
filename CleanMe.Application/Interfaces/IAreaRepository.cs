using CleanMe.Application.ViewModels;
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
        Task<IEnumerable<AreaIndexViewModel>> GetAreaIndexAsync(
            string? regionName, string? name, int? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<AreaViewModel?> GetAreaByIdAsync(int areaId);
        Task<AreaViewModel?> GetAreaViewModelByIdAsync(int areaId);
        Task<Area?> GetAreaViewModelWithAssetLocationsByIdAsync(int areaId);
        Task<AreaViewModel?> PrepareNewAreaViewModelAsync(int regionId);
        Task AddAreaAsync(Area Area, string createdById);
        Task UpdateAreaAsync(Area Area, string updatedById);
        Task SoftDeleteAreaAsync(int areaId, string deletedById);
    }
}