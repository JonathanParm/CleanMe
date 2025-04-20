using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class RegionService : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public RegionService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of regions using Dapper (Optimized for performance)
        public async Task<IEnumerable<RegionIndexViewModel>> GetRegionIndexAsync(
            string? name, string? code, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching regions list using Dapper.");
            try
            {
                var query = "EXEC dbo.RegionGetIndexView @Name, @Code, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    Code = code,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<RegionIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching regions from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<RegionViewModel>> FindDuplicateRegionAsync(string name, string reportCode, int? regionId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Regions WHERE IsDeleted = 0 AND (Name = @name OR ReportCode = @reportCode)";

            if (regionId.HasValue)
            {
                query += " AND regionId != @regionId"; // Exclude a specific region (useful when updating)
            }

            var parameters = new { Name = name, ReportCode = reportCode, regionId = regionId };

            return await _unitOfWork.DapperRepository.QueryAsync<RegionViewModel>(query, parameters);
        }

        public async Task<RegionViewModel?> GetRegionViewModelByIdAsync(int regionId)
        {
            var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(regionId);

            if (region == null)
            {
                return null; // No match found
            }

            // Convert `Region` entity to `RegionViewModel`
            return new RegionViewModel
            {
                regionId = region.regionId,
                Name = region.Name,
                ReportCode = region.ReportCode,
                IsActive = region.IsActive
            };
        }

        public async Task<RegionViewModel?> GetRegionViewModelWithAreasByIdAsync(int regionId)
        {
            var region = await _unitOfWork.RegionRepository.GetRegionWithAreasByIdAsync(regionId);

            if (region == null)
            {
                return null; // No match found
            }

            // Convert `Region` entity to `RegionViewModel`
            return new RegionViewModel
            {
                regionId = region.regionId,
                Name = region.Name,
                ReportCode = region.ReportCode,
                IsActive = region.IsActive,

                AreasList = region.Areas
                    .OrderBy(a => a.SequenceOrder)
                    .Select(a => new AreaIndexViewModel
                    {
                        areaId = a.areaId,
                        Name = a.Name,
                        RegionName = region.Name,
                        ReportCode = a.ReportCode,
                        IsActive = a.IsActive
                    })
                .ToList()
            };
        }

        // Creates a new region (EF Core)
        public async Task<int> AddRegionAsync(RegionViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new regon: {model.Name}");

            var region = new Region
            {
                Name = model.Name,
                ReportCode = model.ReportCode,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.RegionRepository.AddRegionAsync(region);

            return region.regionId;
        }

        // Updates an existing region (EF Core)
        public async Task UpdateRegionAsync(RegionViewModel model, string updatedById)
        {
            var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(model.regionId);
            if (region == null)
            {
                _logger.LogWarning($"Region with ID {model.regionId} not found.");
                throw new Exception("Region not found.");
            }

            region.Name = model.Name;
            region.ReportCode = model.ReportCode;
            region.IsActive = model.IsActive;
            region.UpdatedAt = DateTime.UtcNow;
            region.UpdatedById = updatedById;

            await _unitOfWork.RegionRepository.UpdateRegionAsync(region);
        }

        public async Task<bool> SoftDeleteRegionAsync(int regionId, string updatedById)
        {
            var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(regionId);

            if (region == null)
            {
                throw new KeyNotFoundException("region not found.");
            }

            // Soft delete staff
            region.IsDeleted = true;
            region.IsActive = false;
            region.UpdatedAt = DateTime.UtcNow;
            region.UpdatedById = updatedById;

            await _unitOfWork.RegionRepository.UpdateRegionAsync(region);

            return true;
        }
    }
}
