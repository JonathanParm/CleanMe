using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using CleanMe.Application.DTOs;

namespace CleanMe.Application.Services
{
    public class AmendmentService : IAmendmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetService _assetService;

        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public AmendmentService(
            IUnitOfWork unitOfWork,
            IAssetService assetService,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _assetService = assetService;

            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of Amendments using Dapper (Optimized for performance)
        public async Task<IEnumerable<AmendmentIndexViewModel>> GetAmendmentIndexAsync(
                string? sourceName, string? amendTypeName,
                string? clientName, string? areaName, string? locationName,
                string? mdReference, string? clientReference,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Amendments list using Dapper.");
            try
            {
                var query = "EXEC dbo.AmendmentGetIndexView @SourceName, @AmendTypeName, @ClientName, @AreaName, @LocationName, @MdReference, @ClientReference, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    SourceName = sourceName,
                    AmendTypeName = amendTypeName,
                    ClientName = clientName,
                    AreaName = areaName,
                    LocationName = locationName,
                    MdReference = mdReference,
                    ClientReference = clientReference,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<AmendmentIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Amendments from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<AmendmentViewModel>> FindDuplicateAmendmentAsync(int amendmentTypeId, int assetId, int? excludeAmendmentId = null)
        {
            try
            {
                //_logger.LogInformation($"Finding duplicate Amendments for AmendmentTypeId: {amendmentTypeId}, ExcludeAmendmentId: {excludeAmendmentId}.");

                // Exclude any soft deletes
                var query = "SELECT * FROM Amendments " +
                    "WHERE IsDeleted = 0 " +
                    "AND InvoicedOn IS NULL " +
                    "AND assetId = @assetId " +
                    "AND amendmentTypeId = @amendmentTypeId";

                if (excludeAmendmentId.HasValue)
                {
                    query += " AND amendmentId != @excludeAmendmentId"; // Exclude a specific Amendment (when updating)
                }

                var parameters = new
                {
                    amendmentTypeId = amendmentTypeId,
                    assetId = assetId,
                    excludeAmendmentId = excludeAmendmentId
                };

                return await _unitOfWork.DapperRepository.QueryAsync<AmendmentViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding duplicate Amendments.");
                throw new ApplicationException("Error finding duplicate Amendments", ex);
            }
        }

        public async Task<int?> GetAmendmentLastInvoicedOnByIdAsync(int amendmentId, int amendmentTypeId)
        {
            var query = "SELECT TOP 1 * FROM Amendments WHERE IsDeleted = 0 AND amendmentId = @amendmentId AND amendmentTypeId = @amendmentTypeId AND InvoicedOn IS NOT NULL ORDER BY InvoicedOn DESC";
            var parameters = new { amendmentId = amendmentId, amendmentTypeId = amendmentTypeId };
            var amendment = await _unitOfWork.DapperRepository.QueryFirstOrDefaultAsync<Amendment>(query, parameters);
            if (amendment == null)
            {
                return null; // No match found
            }
            return amendment.amendmentId; // Return the ID of the last invoiced Amendment
        }

        public async Task<AmendmentViewModel?> GetAmendmentViewModelByIdAsync(int amendmentId)
        {
            var amendment = await _unitOfWork.AmendmentRepository.GetAmendmentByIdAsync(amendmentId);

            if (amendment == null)
            {
                return null; // No match found
            }

            //var amendmentType = await _unitOfWork.AmendmentTypeRepository.GetAmendmentTypeByIdAsync(amendment.amendmentTypeId.Value);

            // Convert `Amendment` entity to `AmendmentViewModel`
            return new AmendmentViewModel
            {
                amendmentId = amendment.amendmentId,
                AmendmentSourceName = amendment?.AmendmentSourceName,
                clientId = amendment.clientId,
                ClientName = amendment.Client?.Brand,
                areaId = amendment.areaId,
                AreaName = amendment.Area?.Name,
                assetLocationId = amendment.assetLocationId,
                AssetLocationName = amendment.AssetLocation?.Description,
                assetId = amendment.assetId,
                AssetName = amendment.Asset?.Name,
                amendmentTypeId = amendment.amendmentTypeId,
                AmendmentTypeName = amendment.AmendmentType?.Name,
                cleanFrequencyId = amendment.cleanFrequencyId,
                CleanFrequencyName = amendment.CleanFrequency?.Name,
                itemCodeId = amendment.itemCodeId,
                ItemCodeName = amendment.ItemCode?.Code,
                staffId = amendment.staffId,
                StaffName = amendment.Staff?.Fullname,
                Rate = amendment.Rate,
                Access = amendment.Access,
                IsAccessable = amendment.IsAccessable,
                StartOn = amendment.StartOn,
                FinishOn = amendment.FinishOn,
                Comment = amendment.Comment,
                InvoicedOn = amendment.InvoicedOn,
            };
        }

        public async Task<AmendmentViewModel?> GetAmendmentViewModelOnlyByIdAsync(int amendmentId)
        {
            var amendment = await _unitOfWork.AmendmentRepository.GetAmendmentOnlyByIdAsync(amendmentId);

            if (amendment == null)
            {
                return null; // No match found
            }

            // Convert `Amendment` entity to `AmendmentViewModel`
            return new AmendmentViewModel
            {
                amendmentId = amendment.amendmentId,
                Rate = amendment.Rate,
                Access = amendment.Access,
                IsAccessable = amendment.IsAccessable,
                StartOn = amendment.StartOn,
                FinishOn = amendment.FinishOn,
                Comment = amendment.Comment,
                InvoicedOn = amendment.InvoicedOn
            };
        }

        public async Task<AmendmentTypeHasFieldsDto> GetAmendmentTypeHasFieldsByIdAsync(int amendmentTypeId)
        {
            var amendmentType = await _unitOfWork.AmendmentTypeRepository.GetAmendmentTypeByIdAsync(amendmentTypeId);
            if (amendmentType == null)
            {
                _logger.LogWarning($"AmendmentType with ID {amendmentTypeId} not found.");
                throw new Exception("AmendmentType not found.");
            }
            // Map the fields to the DTO
            return new AmendmentTypeHasFieldsDto
            {
                HasStaffId = amendmentType.HasStaffId,
                HasClientId = amendmentType.HasClientId,
                //HasRegionId = amendmentType.HasRegionId,
                HasAreaId = amendmentType.HasAreaId,
                HasAssetLocationId = amendmentType.HasAssetLocationId,
                HasItemCodeId = amendmentType.HasItemCodeId,
                HasAssetId = amendmentType.HasAssetId,
                HasCleanFrequencyId = amendmentType.HasCleanFrequencyId,
                HasRate = amendmentType.HasRate,
                //HasAccess = amendmentType.HasAccess,
                HasIsAccessable = amendmentType.HasIsAccessable
            };
        }

        //public async Task<AmendmentViewModel> PrepareNewAmendmentViewModelAsync(int regionId)
        //{
        //    var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(regionId);

        //    if (region == null)
        //        throw new Exception("Region not found.");

        //    return new AmendmentViewModel
        //    {
        //        regionId = region.regionId,
        //        RegionName = region.Name,
        //        AssetLocationsList = new List<AssetLocationIndexViewModel>(),
        //        IsActive = true
        //    };
        //}

        // Creates a new Amendment (EF Core)
        public async Task<int> AddAmendmentAsync(AmendmentViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new Amendment: {model.AssetName}");

            try
            {
                int? clientId;
                int? areaId;
                int? assetLocationId;
                int? itemCodeId;

                if (model.assetId.HasValue && model.assetId != 0)
                {
                    var assetHierarchy = await _assetService.GetHierarchyIdsByAssetIdAsync(model.assetId.Value);

                    if (assetHierarchy == null)
                        throw new Exception("Asset not found or missing hierarchy data.");

                    clientId = assetHierarchy.clientId;
                    areaId = assetHierarchy.areaId;
                    assetLocationId = assetHierarchy.assetLocationId;
                    itemCodeId = assetHierarchy.itemCodeId;
                }
                else
                {
                    clientId = model.clientId;
                    areaId = model.areaId;
                    assetLocationId = model.assetLocationId;
                    itemCodeId = model.itemCodeId;
                }

                var amendment = new Amendment
                {
                    amendmentTypeId = model.amendmentTypeId,
                    clientId = clientId == 0 ? null : clientId,
                    areaId = areaId == 0 ? null : areaId,
                    assetLocationId = assetLocationId == 0 ? null : assetLocationId,
                    assetId = model.assetId == 0 ? null : model.assetId,
                    cleanFrequencyId = model.cleanFrequencyId == 0 ? null : model.cleanFrequencyId,
                    itemCodeId = itemCodeId == 0 ? null : itemCodeId,
                    staffId = model.staffId == 0 ? null : model.staffId,
                    Rate = model.Rate,
                    Access = model.Access,
                    IsAccessable = model.IsAccessable,
                    StartOn = model.StartOn,
                    FinishOn = model.FinishOn,
                    Comment = model.Comment,
                    InvoicedOn = model.InvoicedOn,
                    AddedAt = DateTime.UtcNow,
                    AddedById = addedById,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedById = addedById
                };

                await _unitOfWork.AmendmentRepository.AddAmendmentAsync(amendment);

                return amendment.amendmentId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding Amendment.");
                throw new ApplicationException("Error adding Amendment", ex);
            }
        }

        // Updates an existing Amendment (EF Core)
        public async Task UpdateAmendmentAsync(AmendmentViewModel model, string updatedById)
        {
            var amendment = await _unitOfWork.AmendmentRepository.GetAmendmentByIdAsync(model.amendmentId.Value);
            if (amendment == null)
            {
                _logger.LogWarning($"Update Amendment with ID {model.amendmentId} not found.");
                throw new Exception("Amendment not found.");
            }

            int? clientId;
            int? areaId;
            int? assetLocationId;
            int? itemCodeId;

            if (model.assetId.HasValue)
            {
                var assetHierarchy = await _assetService.GetHierarchyIdsByAssetIdAsync(model.assetId.Value);

                if (assetHierarchy == null)
                    throw new Exception("Asset not found or missing hierarchy data.");

                clientId = assetHierarchy.clientId;
                areaId = assetHierarchy.areaId;
                assetLocationId = assetHierarchy.assetLocationId;
                itemCodeId = assetHierarchy.itemCodeId;
            }
            else
            {
                clientId = model.clientId;
                areaId = model.areaId;
                assetLocationId = model.assetLocationId;
                itemCodeId = model.itemCodeId;
            }

            amendment.amendmentTypeId = model.amendmentTypeId;
            amendment.clientId = clientId;
            amendment.areaId = areaId;
            amendment.assetLocationId = assetLocationId;
            amendment.assetId = model.assetId;
            amendment.cleanFrequencyId = model.cleanFrequencyId;
            amendment.itemCodeId = itemCodeId;
            amendment.staffId = model.staffId;
            amendment.Rate = model.Rate;
            amendment.Access = model.Access;
            amendment.IsAccessable = model.IsAccessable;
            amendment.StartOn = model.StartOn;
            amendment.FinishOn = model.FinishOn;
            amendment.Comment = model.Comment;
            amendment.InvoicedOn = model.InvoicedOn;
            amendment.UpdatedAt = DateTime.UtcNow;
            amendment.UpdatedById = updatedById;

            _unitOfWork.AmendmentRepository.UpdateAmendmentAsync(amendment);
        }

        public async Task<bool> SoftDeleteAmendmentAsync(int amendmentId, string updatedById)
        {
            var Amendment = await _unitOfWork.AmendmentRepository.GetAmendmentByIdAsync(amendmentId);

            if (Amendment == null)
            {
                _logger.LogWarning($"Soft Delete Amendment with ID {amendmentId} not found.");
                throw new Exception("Amendment not found.");
            }

            // Soft delete staff
            Amendment.IsDeleted = true;
            Amendment.UpdatedAt = DateTime.UtcNow;
            Amendment.UpdatedById = updatedById;

            _unitOfWork.AmendmentRepository.UpdateAmendmentAsync(Amendment);

            return true;
        }
    }
}
