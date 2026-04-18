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
    public class AssetService : IAssetService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public AssetService(
            IUnitOfWork unitOfWork,

            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;

            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of Assets using Dapper (Optimized for performance)
        public async Task<IEnumerable<AssetIndexViewModel>> GetAssetIndexAsync(
            string? assetName, string? regionName, string? areaName, string? mdReference, string? clientName, string? clientReference,
            string? assetLocation, string? assetType,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Assets list using Dapper.");
            try
            {
                var query = "EXEC dbo.AssetGetIndexView @AssetName, @RegionName, @AreaName, @MdReference, @ClientName, @ClientReference, @AssetLocation, @AssetType, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    AssetName = assetName,
                    RegionName = regionName,
                    AreaName = areaName,
                    MdReference = mdReference,
                    ClientName = clientName,
                    ClientReference = clientReference,
                    AssetLocation = assetLocation,
                    AssetType = assetType,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<AssetIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Assets from stored procedure", ex);
            }
        }

        public async Task<PagedResult<AssetIndexViewModel>> GetPagedIndexAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortOrder,
            string? assetName,
            string? regionName,
            string? areaName,
            string? mdReference,
            string? clientName,
            string? clientReference,
            string? assetLocation,
            string? itemCode)
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
                AssetName = assetName,
                RegionName = regionName,
                AreaName = areaName,
                MdReference = mdReference,
                ClientName = clientName,
                ClientReference = clientReference,
                AssetLocation = assetLocation,
                ItemCode = itemCode
            };

            const string proc = "dbo.AssetGetIndexView";

            var rows = (await _unitOfWork.DapperRepository
                .QueryAsync<AssetIndexRowDTO>(proc, parameters, CommandType.StoredProcedure))
                .ToList();

            var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

            var items = rows.Select(r => new AssetIndexViewModel
            {
                assetId = r.assetId,
                AssetName = r.AssetName,
                RegionName = r.RegionName,
                AreaName = r.AreaName,
                MdReference = r.MdReference,
                ClientName = r.ClientName,
                ClientReference = r.ClientReference,
                AssetLocation = r.AssetLocation,
                ItemCode = r.ItemCode
            }).ToList();

            return new PagedResult<AssetIndexViewModel>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<AssetViewModel>> FindDuplicateAssetAsync(string assetName, int? assetId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Assets WHERE IsDeleted = 0 AND AssetName = @assetName";

            if (assetId.HasValue && assetId > 0)
            {
                query += " AND assetId != @assetId"; // Exclude a specific Asset (useful when updating)
            }

            var parameters = new { AssetName = assetName, assetId = assetId };

            return await _unitOfWork.DapperRepository.QueryAsync<AssetViewModel>(query, parameters);
        }

        public async Task<AssetViewModel?> GetAssetViewModelByIdAsync(int assetId)
        {
            var asset = await _unitOfWork.AssetRepository.GetAssetByIdAsync(assetId);

            if (asset == null)
            {
                return null; // No match found
            }

            // Convert `Asset` entity to `AssetViewModel`
            return new AssetViewModel
            {
                assetId = asset.assetId,
                AssetName = asset.AssetName,
                clientId = asset.clientId,
                ClientName = asset.Client?.Brand,
                assetLocationId = asset.assetLocationId,
                AssetLocationName = asset.AssetLocation?.Description,
                itemCodeId = asset.itemCodeId,
                ItemCodeName = asset.ItemCode?.Code,
                MdReference = asset.MdReference,
                ClientReference = asset.ClientReference,
                Position = asset.Position,
                Access = asset.Access
            };
        }


        public async Task<AssetHierarchyDto?> GetHierarchyIdsByAssetIdAsync(int assetId)
        {
            var query = @"
        SELECT
            ass.assetId,
            ass.AssetName,
            ass.clientId,
            c.Brand AS Client,
            ass.assetLocationId,
            al.[Description] AS AssetLocation,
            al.areaId,
            a.AreaName,
			ass.itemCodeId,
			ic.Code AS ItemName
        FROM 
			dbo.Assets AS ass
            INNER JOIN dbo.Clients AS c ON ass.clientId = c.clientId
            INNER JOIN dbo.AssetLocations AS al ON ass.assetLocationId = al.assetLocationId
            INNER JOIN dbo.Areas AS a ON al.areaId = a.areaId
			INNER JOIN dbo.ItemCodes AS ic ON ass.itemCodeId = ic.itemCodeId
        WHERE 
            ass.assetId = @AssetId";

            var parameters = new { AssetId = assetId };

            return await _unitOfWork.DapperRepository.QueryFirstOrDefaultAsync<AssetHierarchyDto?>(query, parameters);
        }

        public async Task<int> AddAssetAsync(AssetViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new Asset: {model.AssetName}");

            // using .Value to ensure the nullable int is not null
            var Asset = new Asset
            {
                MdReference = model.MdReference,
                AssetName = model.AssetName,
                ClientReference = model.ClientReference,
                Position = model.Position,
                Access = model.Access,
                clientId = model.clientId.Value,
                assetLocationId = model.assetLocationId.Value,
                itemCodeId = model.itemCodeId.Value,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.AssetRepository.AddAssetAsync(Asset);

            return Asset.assetId;
        }

        public async Task UpdateAssetAsync(AssetViewModel model, string updatedById)
        {
            var asset = await _unitOfWork.AssetRepository.GetAssetByIdAsync(model.assetId);
            if (asset == null)
            {
                _logger.LogWarning($"Asset with ID {model.assetId} not found.");
                throw new Exception("Asset not found.");
            }

            asset.MdReference = model.MdReference;
            asset.AssetName = model.AssetName;
            asset.ClientReference = model.ClientReference;
            asset.Position = model.Position;
            asset.Access = model.Access;
            asset.clientId = model.clientId.Value;
            asset.assetLocationId = model.assetLocationId.Value;
            asset.itemCodeId = model.itemCodeId.Value;
            asset.UpdatedAt = DateTime.UtcNow;
            asset.UpdatedById = updatedById;

            await _unitOfWork.AssetRepository.UpdateAssetAsync(asset);
        }

        public async Task<bool> SoftDeleteAssetAsync(int assetId, string updatedById)
        {
            var Asset = await _unitOfWork.AssetRepository.GetAssetByIdAsync(assetId);

            if (Asset == null)
            {
                throw new KeyNotFoundException("Asset not found.");
            }

            // Soft delete staff
            Asset.IsDeleted = true;
            Asset.UpdatedAt = DateTime.UtcNow;
            Asset.UpdatedById = updatedById;

            await _unitOfWork.AssetRepository.UpdateAssetAsync(Asset);

            return true;
        }
    }
}
