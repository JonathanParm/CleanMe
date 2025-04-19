using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Domain.Interfaces
{
    public interface IRegionRepository
    {
        Task<IEnumerable<RegionIndexViewModel>> GetRegionIndexAsync(
                string? name, string? code, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize);
        Task<Region?> GetRegionByIdAsync(int regionId);
        Task<Region?> GetRegionWithAreasByIdAsync(int regionId);
        Task AddRegionAsync(Region region, string createdById);
        Task UpdateRegionAsync(Region region, string updatedById);
        Task SoftDeleteRegionAsync(int regionId, string deletedById);
    }
}