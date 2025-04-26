using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace CleanMe.Application.Services
{
    public class AmendmentService : IAmendmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public AmendmentService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;

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

        public async Task<IEnumerable<AmendmentViewModel>> FindDuplicateAmendmentAsync(int amendmentTypeId, int? excludeamendmentId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Amendments WHERE IsDeleted = 0 AND Invoiced IS NULL AND amendmentTypeId = @amendmentTypeId";

            if (excludeamendmentId.HasValue)
            {
                query += " AND amendmentId != @excludeamendmentId"; // Exclude a specific Amendment (useful when updating)
            }

            var parameters = new { amendmentTypeId = amendmentTypeId, amendmentId = excludeamendmentId };

            return await _unitOfWork.DapperRepository.QueryAsync<AmendmentViewModel>(query, parameters);
        }

        public async Task<int?> GetAmendmentLastInvoicedByIdAsync(int amendmentId, int amendmentTypeId)
        {
            var query = "SELECT TOP 1 * FROM Amendments WHERE IsDeleted = 0 AND amendmentId = @amendmentId AND amendmentTypeId = @amendmentTypeId AND Invoiced IS NOT NULL ORDER BY Invoiced DESC";
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

            // Convert `Amendment` entity to `AmendmentViewModel`
            return new AmendmentViewModel
            {
                amendmentId = amendment.amendmentId,
                AmendmentSourceName = amendment?.AmendmentSourceName,
                ClientName = amendment.Client?.Brand,
                AreaName = amendment.Area?.Name,
                AssetLocationName = amendment.AssetLocation?.Description,
                AssetName = amendment.Asset?.Name,
                CleanFrequencyName = amendment.CleanFrequency?.Name,
                StaffName = amendment.Staff?.Fullname,
                Rate = amendment.Rate,
                Access = amendment.Access,
                IsAccessable = amendment.IsAccessable,
                StartOn = amendment.StartOn,
                FinishOn = amendment.FinishOn,
                Comment = amendment.Comment,
                InvoicedOn = amendment.InvoicedOn
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

            var Amendment = new Amendment
            {
                amendmentTypeId = model.amendmentTypeId,
                clientId = model.clientId,
                areaId = model.areaId,
                assetLocationId = model.assetLocationId,
                assetId = model.assetId,
                cleanFrequencyId = model.cleanFrequencyId,
                staffId = model.staffId,
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

            await _unitOfWork.AmendmentRepository.AddAmendmentAsync(Amendment);

            return Amendment.amendmentId;
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

            amendment.amendmentTypeId = model.amendmentTypeId;
            amendment.clientId = model.clientId;
            amendment.areaId = model.areaId;
            amendment.assetLocationId = model.assetLocationId;
            amendment.assetId = model.assetId;
            amendment.cleanFrequencyId = model.cleanFrequencyId;
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
