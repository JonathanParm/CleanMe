using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class AssetTypeRateService : IAssetTypeRateService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public AssetTypeRateService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;

            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of AssetTypeRates using Dapper (Optimized for performance)
        public async Task<IEnumerable<AssetTypeRateIndexViewModel>> GetAssetTypeRateIndexAsync(
                string? rateName, string? assetTypeName, string? cleanFreqName, string? clientName,
                decimal? rate, string? isDefault, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching AssetTypeRates list using Dapper.");
            try
            {
                var query = "EXEC dbo.AssetTypeRateGetIndexView "
                    + "@RateName, @AssetTypeName, @CleanFreqName, @ClientName, @Rate, @IsDefault, @IsActive, "
                    + "@SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    RateName = rateName,
                    AssetTypeName = assetTypeName,
                    CleanFreqName = cleanFreqName,
                    ClientName = clientName,
                    Rate = rate,
                    IsDefault = isDefault,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<AssetTypeRateIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching AssetTypeRates from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<AssetTypeRateViewModel>> FindDuplicateAssetTypeRateAsync(int assetTypeId, int cleanFrequencyId, int clientId, int? assetTypeRateId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM AssetTypeRates WHERE IsDeleted = 0 "
                + "AND assetTypeId = @assetTypeId "
                + "AND cleanFrequencyId = @cleanFrequencyId "
                + "AND clientId = @clientId ";

            if (assetTypeRateId.HasValue)
            {
                query += " AND assetTypeRateId != @assetTypeRateId"; // Exclude a specific AssetTypeRate (useful when updating)
            }

            var parameters = new { assetTypeId = assetTypeId, cleanFrequencyId = cleanFrequencyId, clientId = clientId, assetTypeRateId = assetTypeRateId };

            return await _unitOfWork.DapperRepository.QueryAsync<AssetTypeRateViewModel>(query, parameters);
        }

        public async Task<AssetTypeRateViewModel?> GetAssetTypeRateViewModelByIdAsync(int assetTypeRateId)
        {
            var AssetTypeRate = await _unitOfWork.AssetTypeRateRepository.GetAssetTypeRateByIdAsync(assetTypeRateId);

            if (AssetTypeRate == null)
            {
                return null; // No match found
            }

            // Convert `AssetTypeRate` entity to `AssetTypeRateViewModel`
            return new AssetTypeRateViewModel
            {
                assetTypeRateId = AssetTypeRate.assetTypeRateId,
                Name = AssetTypeRate.Name,
                assetTypeId = AssetTypeRate.assetTypeId,
                AssetTypeName = AssetTypeRate.AssetType?.Name,
                cleanFrequencyId = AssetTypeRate.cleanFrequencyId,
                CleanFreqName = AssetTypeRate.CleanFrequency?.Name,
                clientId = AssetTypeRate.clientId,
                ClientName = AssetTypeRate.Client?.Brand,
                Rate = AssetTypeRate.Rate,
                IsDefault = AssetTypeRate.IsDefault,
                IsActive = AssetTypeRate.IsActive
            };
        }

        public async Task<AssetTypeRateViewModel?> GetAssetTypeRateViewModelOnlyByIdAsync(int assetTypeRateId)
        {
            var AssetTypeRate = await _unitOfWork.AssetTypeRateRepository.GetAssetTypeRateOnlyByIdAsync(assetTypeRateId);

            if (AssetTypeRate == null)
            {
                return null; // No match found
            }

            // Convert `AssetTypeRate` entity to `AssetTypeRateViewModel`
            return new AssetTypeRateViewModel
            {
                assetTypeRateId = AssetTypeRate.assetTypeRateId,
                Name = AssetTypeRate.Name,
                assetTypeId = AssetTypeRate.assetTypeId,
                //AssetTypeName = AssetTypeRate.AssetType?.Name,
                cleanFrequencyId = AssetTypeRate.cleanFrequencyId,
                //CleanFreqName = AssetTypeRate.CleanFrequency?.Name,
                clientId = AssetTypeRate.clientId,
                //ClientName = AssetTypeRate.Client?.Brand,
                Rate = AssetTypeRate.Rate,
                IsDefault = AssetTypeRate.IsDefault,
                IsActive = AssetTypeRate.IsActive,
            };
        }

        //public async Task<AssetTypeRateViewModel> PrepareNewAssetTypeRateViewModelAsync(int regionId)
        //{
        //    var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(regionId);

        //    if (region == null)
        //        throw new Exception("Region not found.");

        //    return new AssetTypeRateViewModel
        //    {
        //        regionId = region.regionId,
        //        RegionName = region.Name,
        //        AssetLocationsList = new List<AssetLocationIndexViewModel>(),
        //        IsActive = true
        //    };
        //}

        // Creates a new AssetTypeRate (EF Core)
        public async Task<int> AddAssetTypeRateAsync(AssetTypeRateViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new AssetTypeRate: {model.Name}");

            var AssetTypeRate = new AssetTypeRate
            {
                Name = model.Name,
                assetTypeId = model.assetTypeId.Value,
                cleanFrequencyId = model.cleanFrequencyId.Value,
                clientId = model.clientId.Value,
                Rate = model.Rate,
                IsDefault = model.IsDefault,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.AssetTypeRateRepository.AddAssetTypeRateAsync(AssetTypeRate);

            return AssetTypeRate.assetTypeRateId;
        }

        // Updates an existing AssetTypeRate (EF Core)
        public async Task UpdateAssetTypeRateAsync(AssetTypeRateViewModel model, string updatedById)
        {
            var AssetTypeRate = await _unitOfWork.AssetTypeRateRepository.GetAssetTypeRateOnlyByIdAsync(model.assetTypeRateId);
            if (AssetTypeRate == null)
            {
                _logger.LogWarning($"Update AssetTypeRate with ID {model.assetTypeRateId} not found.");
                throw new Exception("AssetTypeRate not found.");
            }

            AssetTypeRate.Name = model.Name;
            AssetTypeRate.assetTypeId = model.assetTypeId.Value;
            AssetTypeRate.cleanFrequencyId = model.cleanFrequencyId.Value;
            AssetTypeRate.clientId = model.clientId.Value;
            AssetTypeRate.Rate = model.Rate;
            AssetTypeRate.IsDefault = model.IsDefault;
            AssetTypeRate.IsActive = model.IsActive;
            AssetTypeRate.UpdatedAt = DateTime.UtcNow;
            AssetTypeRate.UpdatedById = updatedById;

            _unitOfWork.AssetTypeRateRepository.UpdateAssetTypeRateAsync(AssetTypeRate);
        }

        public async Task<bool> SoftDeleteAssetTypeRateAsync(int assetTypeRateId, string updatedById)
        {
            var AssetTypeRate = await _unitOfWork.AssetTypeRateRepository.GetAssetTypeRateOnlyByIdAsync(assetTypeRateId);

            if (AssetTypeRate == null)
            {
                _logger.LogWarning($"Soft Delete AssetTypeRate with ID {assetTypeRateId} not found.");
                throw new Exception("AssetTypeRate not found.");
            }

            // Soft delete staff
            AssetTypeRate.IsDeleted = true;
            AssetTypeRate.IsActive = false;
            AssetTypeRate.UpdatedAt = DateTime.UtcNow;
            AssetTypeRate.UpdatedById = updatedById;

            _unitOfWork.AssetTypeRateRepository.UpdateAssetTypeRateAsync(AssetTypeRate);

            return true;
        }
    }
}
