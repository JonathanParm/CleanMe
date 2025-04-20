using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

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
            string? regionName, string? name, int? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Areas list using Dapper.");
            try
            {
                var query = "EXEC dbo.AreaGetIndexView @RegionName, @Name, @ReportCode, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    RegionName = regionName,
                    Name = name,
                    ReportCode = reportCode,
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

        public async Task<IEnumerable<AreaViewModel>> FindDuplicateAreaAsync(string name, int reportCode, int? areaId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Areas WHERE IsDeleted = 0 AND (Name = @name OR ReportCode = @reportCode)";

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
                Name = area.Name,
                regionId = area.regionId,
                RegionName = area.Region?.Name,
                SequenceOrder = area.SequenceOrder,
                SeqNo = area.SeqNo,
                ReportCode = area.ReportCode,
                IsActive = area.IsActive
            };
        }

        public async Task<AreaViewModel?> GetAreaViewModelWithAssetLocationsByIdAsync(int areaId)
        {
            var area = await _unitOfWork.AreaRepository.GetAreaWithAssetLocationsByIdAsync(areaId);

            if (area == null)
            {
                return null; // No match found
            }

            // Convert `Area` entity to `AreaViewModel`
            return new AreaViewModel
            {
                areaId = area.areaId,
                Name = area.Name,
                regionId = area.regionId,
                RegionName = area.Region?.Name,
                SequenceOrder = area.SequenceOrder,
                SeqNo = area.SeqNo,
                ReportCode = area.ReportCode,
                IsActive = area.IsActive,
                AssetLocationsList = area.AssetLocations
                .OrderBy(al => al.SequenceOrder)
                .Select(al => new AssetLocationIndexViewModel
                {
                    assetLocationId = al.assetLocationId,
                    Description = al.Description,
                    ReportCode = al.ReportCode,
                    TownSuburb = string.Join(", ", new[] { al.Address.TownOrCity, al.Address.Suburb }
    .Where(s => !string.IsNullOrWhiteSpace(s))),
                    IsActive = al.IsActive
                })
                .ToList()
            };
        }

        public async Task<AreaViewModel> PrepareNewAreaViewModelAsync(int regionId)
        {
            var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(regionId);

            if (region == null)
                throw new Exception("Region not found.");

            return new AreaViewModel
            {
                regionId = region.regionId,
                RegionName = region.Name,
                AssetLocationsList = new List<AssetLocationIndexViewModel>(),
                IsActive = true
            };
        }

        // Creates a new Area (EF Core)
        public async Task<int> AddAreaAsync(AreaViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new area: {model.Name}");

            var Area = new Area
            {
                Name = model.Name,
                regionId = model.regionId,
                ReportCode = model.ReportCode,
                SequenceOrder = model.SequenceOrder,
                SeqNo = model.SeqNo,
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

            Area.Name = model.Name;
            Area.regionId = model.regionId;
            Area.ReportCode = model.ReportCode;
            Area.SequenceOrder = model.SequenceOrder;
            Area.SeqNo = model.SeqNo;
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
