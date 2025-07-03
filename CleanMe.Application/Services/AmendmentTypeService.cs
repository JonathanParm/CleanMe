using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class AmendmentTypeService : IAmendmentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public AmendmentTypeService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of AmendmentTypes using Dapper (Optimized for performance)
        public async Task<IEnumerable<AmendmentTypeIndexViewModel>> GetAmendmentTypeIndexAsync(
            string? name, string? description, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Amendment types list using Dapper.");
            try
            {
                var query = "EXEC dbo.AmendmentTypeGetIndexView @Name, @Description, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    Description = description,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<AmendmentTypeIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Amendment types from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<AmendmentTypeViewModel>> FindDuplicateAmendmentTypeAsync(string name, int? amendmentTypeId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM AmendmentTypes WHERE IsDeleted = 0 AND Name = @name";

            if (amendmentTypeId.HasValue)
            {
                query += " AND amendmentTypeId != @amendmentTypeId"; // Exclude a specific AmendmentType (useful when updating)
            }

            var parameters = new { Name = name, amendmentTypeId = amendmentTypeId };

            return await _unitOfWork.DapperRepository.QueryAsync<AmendmentTypeViewModel>(query, parameters);
        }

        public async Task<AmendmentTypeViewModel?> GetAmendmentTypeViewModelByIdAsync(int amendmentTypeId)
        {
            var AmendmentType = await _unitOfWork.AmendmentTypeRepository.GetAmendmentTypeByIdAsync(amendmentTypeId);

            if (AmendmentType == null)
            {
                return null; // No match found
            }

            // Convert `AmendmentType` entity to `AmendmentTypeViewModel`
            return new AmendmentTypeViewModel
            {
                amendmentTypeId = AmendmentType.amendmentTypeId,
                Name = AmendmentType.Name,
                Description = AmendmentType.Description,
                SortOrder = AmendmentType.SortOrder,
                HasStaffId = AmendmentType.HasStaffId,
                HasClientId = AmendmentType.HasClientId,
                //HasRegionId = AmendmentType.HasRegionId,
                HasAreaId = AmendmentType.HasAreaId,
                HasAssetLocationId = AmendmentType.HasAssetLocationId,
                HasItemCodeId = AmendmentType.HasItemCodeId,
                HasAssetId = AmendmentType.HasAssetId,
                HasCleanFrequencyId = AmendmentType.HasCleanFrequencyId,
                HasRate = AmendmentType.HasRate,
                //HasAccess = AmendmentType.HasAccess,
                HasIsAccessable = AmendmentType.HasIsAccessable,
                IsActive = AmendmentType.IsActive
            };
        }

        //public async Task<AmendmentTypeViewModel?> GetAmendmentTypeViewModelWithAssetTypesByIdAsync(int amendmentTypeId)
        //{
        //    var AmendmentType = await _unitOfWork.AmendmentTypeRepository.GetAmendmentTypeViewModelWithAmendmentsByIdAsync(amendmentTypeId);

        //    if (AmendmentType == null)
        //    {
        //        return null; // No match found
        //    }

        //    // Convert `AmendmentType` entity to `AmendmentTypeViewModel`
        //    return new AmendmentTypeViewModel
        //    {
        //        amendmentTypeId = AmendmentType.amendmentTypeId,
        //        Name = AmendmentType.Name,
        //        Description = AmendmentType.Description,
        //        IsActive = AmendmentType.IsActive,

        //        AssetTypesList = AmendmentType.AssetTypes
        //            .OrderBy(a => a.Name)
        //            .Select(a => new AssetTypeIndexViewModel
        //            {
        //                assetTypeId = a.assetTypeId,
        //                Name = a.Name,
        //                IsActive = a.IsActive
        //            })
        //        .ToList()
        //    };
        //}

        // Creates a new AmendmentType (EF Core)
        public async Task<int> AddAmendmentTypeAsync(AmendmentTypeViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new amendment type: {model.Name}");

            var AmendmentType = new AmendmentType
            {
                Name = model.Name,
                Description = model.Description,
                SortOrder = model.SortOrder,
                HasStaffId = model.HasStaffId,
                HasClientId = model.HasClientId,
                //HasRegionId = model.HasRegionId,
                HasAreaId = model.HasAreaId,
                HasAssetLocationId = model.HasAssetLocationId,
                HasItemCodeId = model.HasItemCodeId,
                HasAssetId = model.HasAssetId,
                HasCleanFrequencyId = model.HasCleanFrequencyId,
                HasRate = model.HasRate,
                //HasAccess = model.HasAccess,
                HasIsAccessable = model.HasIsAccessable,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.AmendmentTypeRepository.AddAmendmentTypeAsync(AmendmentType);

            return AmendmentType.amendmentTypeId;
        }

        // Updates an existing AmendmentType (EF Core)
        public async Task UpdateAmendmentTypeAsync(AmendmentTypeViewModel model, string updatedById)
        {
            var AmendmentType = await _unitOfWork.AmendmentTypeRepository.GetAmendmentTypeByIdAsync(model.amendmentTypeId);
            if (AmendmentType == null)
            {
                _logger.LogWarning($"amendment type with ID {model.amendmentTypeId} not found.");
                throw new Exception("amendment type not found.");
            }

            AmendmentType.Name = model.Name;
            AmendmentType.Description = model.Description;
            AmendmentType.SortOrder = model.SortOrder;
            AmendmentType.HasStaffId = model.HasStaffId;
            AmendmentType.HasClientId = model.HasClientId;
            //AmendmentType.HasRegionId = model.HasRegionId;
            AmendmentType.HasAreaId = model.HasAreaId;
            AmendmentType.HasAssetLocationId = model.HasAssetLocationId;
            AmendmentType.HasItemCodeId = model.HasItemCodeId;
            AmendmentType.HasAssetId = model.HasAssetId;
            AmendmentType.HasCleanFrequencyId = model.HasCleanFrequencyId;
            AmendmentType.HasRate = model.HasRate;
            //AmendmentType.HasAccess = model.HasAccess;
            AmendmentType.HasIsAccessable = model.HasIsAccessable;
            AmendmentType.IsActive = model.IsActive;
            AmendmentType.UpdatedAt = DateTime.UtcNow;
            AmendmentType.UpdatedById = updatedById;

            await _unitOfWork.AmendmentTypeRepository.UpdateAmendmentTypeAsync(AmendmentType);
        }

        public async Task<bool> SoftDeleteAmendmentTypeAsync(int amendmentTypeId, string updatedById)
        {
            var AmendmentType = await _unitOfWork.AmendmentTypeRepository.GetAmendmentTypeByIdAsync(amendmentTypeId);

            if (AmendmentType == null)
            {
                throw new KeyNotFoundException("amendment type not found.");
            }

            // Soft delete staff
            AmendmentType.IsDeleted = true;
            AmendmentType.IsActive = false;
            AmendmentType.UpdatedAt = DateTime.UtcNow;
            AmendmentType.UpdatedById = updatedById;

            await _unitOfWork.AmendmentTypeRepository.UpdateAmendmentTypeAsync(AmendmentType);

            return true;
        }
    }
}
