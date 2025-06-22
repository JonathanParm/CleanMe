using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class ItemCodeRateService : IItemCodeRateService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public ItemCodeRateService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;

            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of ItemCodeRates using Dapper (Optimized for performance)
        public async Task<IEnumerable<ItemCodeRateIndexViewModel>> GetItemCodeRateIndexAsync(
                string? rateName, string? itemCodeName, string? cleanFreqName, string? clientName,
                decimal? rate, string? isDefault, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Item Code Rates list using Dapper.");
            try
            {
                var query = "EXEC dbo.ItemCodeRateGetIndexView "
                    + "@RateName, @ItemCodeName, @CleanFreqName, @ClientName, @Rate, @IsDefault, @IsActive, "
                    + "@SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    RateName = rateName,
                    ItemCodeName = itemCodeName,
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

                return await _unitOfWork.DapperRepository.QueryAsync<ItemCodeRateIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Item Code Rates from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<ItemCodeRateViewModel>> FindDuplicateItemCodeRateAsync(int itemCodeId, int cleanFrequencyId, int clientId, int? itemCodeRateId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM ItemCodeRates WHERE IsDeleted = 0 "
                + "AND itemCodeId = @itemCodeId "
                + "AND cleanFrequencyId = @cleanFrequencyId "
                + "AND clientId = @clientId ";

            if (itemCodeRateId.HasValue)
            {
                query += " AND itemCodeRateId != @itemCodeRateId"; // Exclude a specific ItemCodeRate (useful when updating)
            }

            var parameters = new { itemCodeId = itemCodeId, cleanFrequencyId = cleanFrequencyId, clientId = clientId, itemCodeRateId = itemCodeRateId };

            return await _unitOfWork.DapperRepository.QueryAsync<ItemCodeRateViewModel>(query, parameters);
        }

        public async Task<ItemCodeRateViewModel?> GetItemCodeRateViewModelByIdAsync(int itemCodeRateId)
        {
            var ItemCodeRate = await _unitOfWork.ItemCodeRateRepository.GetItemCodeRateByIdAsync(itemCodeRateId);

            if (ItemCodeRate == null)
            {
                return null; // No match found
            }

            // Convert `ItemCodeRate` entity to `ItemCodeRateViewModel`
            return new ItemCodeRateViewModel
            {
                itemCodeRateId = ItemCodeRate.itemCodeRateId,
                Name = ItemCodeRate.Name,
                itemCodeId = ItemCodeRate.itemCodeId,
                ItemCodeName = ItemCodeRate.ItemCode?.Code,
                cleanFrequencyId = ItemCodeRate.cleanFrequencyId,
                CleanFreqName = ItemCodeRate.CleanFrequency?.Name,
                clientId = ItemCodeRate.clientId,
                ClientName = ItemCodeRate.Client?.Brand,
                Rate = ItemCodeRate.Rate,
                IsDefault = ItemCodeRate.IsDefault,
                IsActive = ItemCodeRate.IsActive
            };
        }

        public async Task<ItemCodeRateViewModel?> GetItemCodeRateViewModelOnlyByIdAsync(int itemCodeRateId)
        {
            var ItemCodeRate = await _unitOfWork.ItemCodeRateRepository.GetItemCodeRateOnlyByIdAsync(itemCodeRateId);

            if (ItemCodeRate == null)
            {
                return null; // No match found
            }

            // Convert `ItemCodeRate` entity to `ItemCodeRateViewModel`
            return new ItemCodeRateViewModel
            {
                itemCodeRateId = ItemCodeRate.itemCodeRateId,
                Name = ItemCodeRate.Name,
                itemCodeId = ItemCodeRate.itemCodeId,
                ItemCodeName = ItemCodeRate.ItemCode?.Code,
                cleanFrequencyId = ItemCodeRate.cleanFrequencyId,
                CleanFreqName = ItemCodeRate.CleanFrequency?.Name,
                clientId = ItemCodeRate.clientId,
                ClientName = ItemCodeRate.Client?.Brand,
                Rate = ItemCodeRate.Rate,
                IsDefault = ItemCodeRate.IsDefault,
                IsActive = ItemCodeRate.IsActive,
            };
        }

        //public async Task<ItemCodeRateViewModel> PrepareNewItemCodeRateViewModelAsync(int regionId)
        //{
        //    var region = await _unitOfWork.RegionRepository.GetRegionByIdAsync(regionId);

        //    if (region == null)
        //        throw new Exception("Region not found.");

        //    return new ItemCodeRateViewModel
        //    {
        //        regionId = region.regionId,
        //        RegionName = region.Name,
        //        AssetLocationsList = new List<AssetLocationIndexViewModel>(),
        //        IsActive = true
        //    };
        //}

        // Creates a new ItemCodeRate (EF Core)
        public async Task<int> AddItemCodeRateAsync(ItemCodeRateViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new ItemCodeRate: {model.Name}");

            var ItemCodeRate = new ItemCodeRate
            {
                Name = model.Name,
                itemCodeId = model.itemCodeId.Value,
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

            await _unitOfWork.ItemCodeRateRepository.AddItemCodeRateAsync(ItemCodeRate);

            return ItemCodeRate.itemCodeRateId;
        }

        // Updates an existing ItemCodeRate (EF Core)
        public async Task UpdateItemCodeRateAsync(ItemCodeRateViewModel model, string updatedById)
        {
            var ItemCodeRate = await _unitOfWork.ItemCodeRateRepository.GetItemCodeRateOnlyByIdAsync(model.itemCodeRateId);
            if (ItemCodeRate == null)
            {
                _logger.LogWarning($"Update Item Code Rate with ID {model.itemCodeRateId} not found.");
                throw new Exception("Item Code Rate not found.");
            }

            ItemCodeRate.Name = model.Name;
            ItemCodeRate.itemCodeId = model.itemCodeId.Value;
            ItemCodeRate.cleanFrequencyId = model.cleanFrequencyId.Value;
            ItemCodeRate.clientId = model.clientId.Value;
            ItemCodeRate.Rate = model.Rate;
            ItemCodeRate.IsDefault = model.IsDefault;
            ItemCodeRate.IsActive = model.IsActive;
            ItemCodeRate.UpdatedAt = DateTime.UtcNow;
            ItemCodeRate.UpdatedById = updatedById;

            _unitOfWork.ItemCodeRateRepository.UpdateItemCodeRateAsync(ItemCodeRate);
        }

        public async Task<bool> SoftDeleteItemCodeRateAsync(int itemCodeRateId, string updatedById)
        {
            var ItemCodeRate = await _unitOfWork.ItemCodeRateRepository.GetItemCodeRateOnlyByIdAsync(itemCodeRateId);

            if (ItemCodeRate == null)
            {
                _logger.LogWarning($"Soft Delete ItemCodeRate with ID {itemCodeRateId} not found.");
                throw new Exception("ItemCodeRate not found.");
            }

            // Soft delete staff
            ItemCodeRate.IsDeleted = true;
            ItemCodeRate.IsActive = false;
            ItemCodeRate.UpdatedAt = DateTime.UtcNow;
            ItemCodeRate.UpdatedById = updatedById;

            _unitOfWork.ItemCodeRateRepository.UpdateItemCodeRateAsync(ItemCodeRate);

            return true;
        }
    }
}
