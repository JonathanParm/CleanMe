using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class AssetTypeService : IAssetTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public AssetTypeService(
            IUnitOfWork unitOfWork,

            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;

            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of AssetTypes using Dapper (Optimized for performance)
        public async Task<IEnumerable<AssetTypeIndexViewModel>> GetAssetTypeIndexAsync(
            string? name, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Asset Types list using Dapper.");
            try
            {
                var query = "EXEC dbo.AssetTypeGetIndexView @Name, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<AssetTypeIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Asset Types from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<AssetTypeViewModel>> FindDuplicateAssetTypeAsync(string name, int? assetTypeId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM AssetTypes WHERE IsDeleted = 0 AND Name = @name";

            if (assetTypeId.HasValue)
            {
                query += " AND assetTypeId != @AssetTypeId"; // Exclude a specific AssetType (useful when updating)
            }

            var parameters = new { Name = name, AssetTypeId = assetTypeId };

            return await _unitOfWork.DapperRepository.QueryAsync<AssetTypeViewModel>(query, parameters);
        }

        public async Task<AssetTypeViewModel?> GetAssetTypeViewModelByIdAsync(int assetTypeId)
        {
            var assetType = await _unitOfWork.AssetTypeRepository.GetAssetTypeByIdAsync(assetTypeId);

            if (assetType == null)
            {
                return null; // No match found
            }

            // Convert `AssetType` entity to `AssetTypeViewModel`
            return new AssetTypeViewModel
            {
                assetTypeId = assetType.assetTypeId,
                Name = assetType.Name,
                IsActive = assetType.IsActive
            };
        }

        // Creates a new AssetType (EF Core)
        public async Task<int> AddAssetTypeAsync(AssetTypeViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new asset type: {model.Name}");

            var AssetType = new AssetType
            {
                Name = model.Name,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.AssetTypeRepository.AddAssetTypeAsync(AssetType);

            return AssetType.assetTypeId;
        }

        // Updates an existing AssetType (EF Core)
        public async Task UpdateAssetTypeAsync(AssetTypeViewModel model, string updatedById)
        {
            var AssetType = await _unitOfWork.AssetTypeRepository.GetAssetTypeByIdAsync(model.assetTypeId);
            if (AssetType == null)
            {
                _logger.LogWarning($"Asset Type with ID {model.assetTypeId} not found.");
                throw new Exception("Asset Type not found.");
            }

            AssetType.Name = model.Name;
            AssetType.IsActive = model.IsActive;
            AssetType.UpdatedAt = DateTime.UtcNow;
            AssetType.UpdatedById = updatedById;

            await _unitOfWork.AssetTypeRepository.UpdateAssetTypeAsync(AssetType);
        }

        public async Task<bool> SoftDeleteAssetTypeAsync(int assetTypeId, string updatedById)
        {
            var AssetType = await _unitOfWork.AssetTypeRepository.GetAssetTypeByIdAsync(assetTypeId);

            if (AssetType == null)
            {
                throw new KeyNotFoundException("Asset Type not found.");
            }

            // Soft delete staff
            AssetType.IsDeleted = true;
            AssetType.IsActive = false;
            AssetType.UpdatedAt = DateTime.UtcNow;
            AssetType.UpdatedById = updatedById;

            await _unitOfWork.AssetTypeRepository.UpdateAssetTypeAsync(AssetType);

            return true;
        }
    }
}
