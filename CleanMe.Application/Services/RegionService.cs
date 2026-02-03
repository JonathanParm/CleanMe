using CleanMe.Application.DTOs;
using CleanMe.Application.Helpers.Paging;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;

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
        public async Task<PagedResult<RegionIndexViewModel>> GetPagedIndexAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortOrder,
            string? regionName,
            string? reportCode,
            string? isActive)
        {
            // Defensive normalisation (helps prevent bad sort inputs)
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 5 : pageSize;

            sortOrder = (sortOrder?.ToUpperInvariant() == "DESC") ? "DESC" : "ASC";
            sortColumn = string.IsNullOrWhiteSpace(sortColumn) ? "AssetName" : sortColumn;

            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                RegionName = regionName,
                ReportCode = reportCode,
                IsActive = isActive
            };

            const string proc = "dbo.RegionGetIndexView";

            var rows = (await _unitOfWork.DapperRepository
                .QueryAsync<RegionIndexRowDTO>(proc, parameters, CommandType.StoredProcedure))
                .ToList();

            var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

            var items = rows.Select(r => new RegionIndexViewModel
            {
                regionId = r.regionId,
                RegionName = r.RegionName,
                ReportCode = r.ReportCode,
                IsActive = r.IsActive
            }).ToList();

            return new PagedResult<RegionIndexViewModel>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        public async Task<IEnumerable<RegionViewModel>> FindDuplicateRegionAsync(string name, string reportCode, int? regionId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Regions WHERE IsDeleted = 0 AND (RegionName = @name OR ReportCode = @reportCode)";

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
                RegionName = region.RegionName,
                ReportCode = region.ReportCode,
                IsActive = region.IsActive
            };
        }

        public async Task<RegionWithAreasViewModel?> GetRegionWithAreasViewModelByIdAsync(int regionId, int pageNumber, int pageSize)
        {
            var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(regionId);
            if (region == null)
                return null;

            var totalCount =
                await _unitOfWork.RegionRepository.GetRegionAreaCountAsync(regionId);

            var areas =
                await _unitOfWork.RegionRepository.GetRegionAreasPagedAsync(
                    regionId, pageNumber, pageSize);

            return new RegionWithAreasViewModel
            {
                Region = region,
                Areas = areas,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // Creates a new region (EF Core)
        public async Task<int> AddRegionAsync(RegionViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new regon: {model.RegionName}");

            var region = new Region
            {
                RegionName = model.RegionName,
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

            region.RegionName = model.RegionName;
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
