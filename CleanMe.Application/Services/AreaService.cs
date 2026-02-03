using CleanMe.Application.DTOs;
using CleanMe.Application.Helpers.Paging;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMe.Application.Services
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public AreaService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;

            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of Areas using Dapper (Optimized for performance)
        public async Task<IEnumerable<AreaIndexViewModel>> GetAreaIndexAsync(
            string? regionName, string? name, int? reportCode, string? cleanerName, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Areas list using Dapper.");
            try
            {
                var query = "EXEC dbo.AreaGetIndexView @RegionName, @Name, @ReportCode, @CleanerName, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    RegionName = regionName,
                    Name = name,
                    ReportCode = reportCode,
                    CleanerName = cleanerName,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<AreaIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Areas from stored procedure", ex);
            }
        }
        public async Task<PagedResult<AreaIndexViewModel>> GetPagedIndexAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortOrder,
            string? regionName,
            string? areaName,
            int? reportCode,
            string? cleanerName,
            string? isActive)
        {
            // Defensive normalisation (helps prevent bad sort inputs)
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 20 : pageSize;

            sortOrder = (sortOrder?.ToUpperInvariant() == "DESC") ? "DESC" : "ASC";
            sortColumn = string.IsNullOrWhiteSpace(sortColumn) ? "AreaName" : sortColumn;

            var parameters = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                RegionName = regionName,
                AreaName = areaName,
                ReportCode = reportCode,
                CleanerName = cleanerName,
                IsActive = isActive
            };

            const string proc = "dbo.AreaGetIndexView";

            var rows = (await _unitOfWork.DapperRepository
                .QueryAsync<AreaIndexRowDTO>(proc, parameters, CommandType.StoredProcedure))
                .ToList();

            var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

            var items = rows.Select(r => new AreaIndexViewModel
            {
                areaId = r.areaId,
                RegionName = r.RegionName,
                AreaName = r.AreaName,
                ReportCode = r.ReportCode,
                CleanerName = r.CleanerName,
                IsActive = r.IsActive
            }).ToList();

            return new PagedResult<AreaIndexViewModel>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<AreaViewModel>> FindDuplicateAreaAsync(string name, int reportCode, int? areaId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Areas WHERE IsDeleted = 0 AND (AreaName = @name OR ReportCode = @reportCode)";

            if (areaId.HasValue)
            {
                query += " AND areaId != @areaId"; // Exclude a specific Area (useful when updating)
            }

            var parameters = new { Name = name, ReportCode = reportCode, areaId = areaId };

            return await _unitOfWork.DapperRepository.QueryAsync<AreaViewModel>(query, parameters);
        }

        public async Task<AreaViewModel?> GetAreaViewModelByIdAsync(int areaId)
        {
            var area = await _unitOfWork.AreaRepository.GetAreaByIdAsync(areaId);

            if (area == null)
            {
                return null; // No match found
            }

            // Convert `Area` entity to `AreaViewModel`
            return new AreaViewModel
            {
                areaId = area.areaId,
                AreaName = area.AreaName,
                regionId = area.regionId,
                RegionName = area.Region?.RegionName,
                SortOrder = area.SortOrder,
                ReportCode = area.ReportCode,
                IsActive = area.IsActive
            };
        }

        //    public async Task<AreaViewModel?> GetAreaViewModelWithAssetLocationsByIdAsync(int areaId)
        //    {
        //        var area = await _unitOfWork.AreaRepository.GetAreaWithAssetLocationsByIdAsync(areaId);

        //        if (area == null)
        //        {
        //            return null; // No match found
        //        }

        //        // Convert `Area` entity to `AreaViewModel`
        //        return new AreaViewModel
        //        {
        //            areaId = area.areaId,
        //            AreaName = area.AreaName,
        //            regionId = area.regionId,
        //            RegionName = area.Region?.RegionName,
        //            SortOrder = area.SortOrder,
        //            ReportCode = area.ReportCode,
        //            IsActive = area.IsActive,
        //            AssetLocationsList = area.AssetLocations
        //            .OrderBy(al => al.SortOrder)
        //            .Select(al => new AssetLocationIndexViewModel
        //            {
        //                assetLocationId = al.assetLocationId,
        //                Description = al.Description,
        //                ReportCode = al.ReportCode,
        //                TownSuburb = string.Join(", ", new[] { al.Address.TownOrCity, al.Address.Suburb }
        //.Where(s => !string.IsNullOrWhiteSpace(s))),
        //                IsActive = al.IsActive
        //            })
        //            .ToList()
        //        };
        //    }

        public async Task<AreaWithAssetLocationsViewModel?> GetAreaWithAssetLocationsViewModelByIdAsync(int areaId, int pageNumber, int pageSize)
        {
            var area = await _unitOfWork.AreaRepository.GetAreaByIdAsync(areaId);
            if (area == null)
                return null;

            var totalCount =
                await _unitOfWork.AreaRepository.GetAreaAssetLocationsCountAsync(areaId);

            var assetLocations =
                await _unitOfWork.AreaRepository.GetAreaAssetLocationsPagedAsync(
                    areaId, pageNumber, pageSize);

            return new AreaWithAssetLocationsViewModel
            {
                Area = area,
                AssetLocations = area.AssetLocations
                    .OrderBy(al => al.SortOrder)
                    .Select(al => new AssetLocationIndexViewModel
                    {
                        assetLocationId = al.assetLocationId,
                        Description = al.Description,
                        ReportCode = al.ReportCode,
                        TownSuburb = string.Join(", ", new[] { al.Address.TownOrCity, al.Address.Suburb }
        .Where(s => !string.IsNullOrWhiteSpace(s))),
                        IsActive = al.IsActive
                    })
                    .ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<AreaWithAssetLocationsViewModel> PrepareNewAreaViewModelAsync(int regionId, int pageNumber, int pageSize)
        {
            var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(regionId);
            if (region == null)
                throw new Exception("Region not found.");

            var regions = await _unitOfWork.RegionRepository.GetAllRegionsAsync();

            return new AreaWithAssetLocationsViewModel
            {
                Area = new Area
                {
                    regionId = region.regionId,
                    IsActive = true
                },

                Regions = regions
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.RegionName)
                .Select(r => new SelectListItem
                {
                    Value = r.regionId.ToString(),
                    Text = r.RegionName,
                    Selected = (r.regionId == region.regionId)
                })
                .ToList(),

                AssetLocations = Array.Empty<AssetLocationIndexViewModel>(),

                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0
            };
        }


        // Creates a new Area (EF Core)
        public async Task<int> AddAreaAsync(AreaViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new area: {model.AreaName}");

            var Area = new Area
            {
                AreaName = model.AreaName,
                regionId = model.regionId,
                ReportCode = model.ReportCode,
                SortOrder = model.SortOrder,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.AreaRepository.AddAreaAsync(Area);

            return Area.areaId;
        }

        // Updates an existing Area (EF Core)
        public async Task UpdateAreaAsync(AreaViewModel model, string updatedById)
        {
            var Area = await _unitOfWork.AreaRepository.GetAreaByIdAsync(model.areaId);
            if (Area == null)
            {
                _logger.LogWarning($"Update Area with ID {model.areaId} not found.");
                throw new Exception("Area not found.");
            }

            Area.AreaName = model.AreaName;
            Area.regionId = model.regionId;
            Area.ReportCode = model.ReportCode;
            Area.SortOrder = model.SortOrder;
            Area.IsActive = model.IsActive;
            Area.UpdatedAt = DateTime.UtcNow;
            Area.UpdatedById = updatedById;

            _unitOfWork.AreaRepository.UpdateAreaAsync(Area);
        }

        public async Task<bool> SoftDeleteAreaAsync(int areaId, string updatedById)
        {
            var area = await _unitOfWork.AreaRepository.GetAreaByIdAsync(areaId);

            if (area == null)
            {
                _logger.LogWarning($"Soft Delete Area with ID {areaId} not found.");
                throw new Exception("Area not found.");
            }

            // Soft delete staff
            area.IsDeleted = true;
            area.IsActive = false;
            area.UpdatedAt = DateTime.UtcNow;
            area.UpdatedById = updatedById;

            _unitOfWork.AreaRepository.UpdateAreaAsync(area);

            return true;
        }
    }
}
