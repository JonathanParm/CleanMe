using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class ItemCodeService : IItemCodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public ItemCodeService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of ItemCodes using Dapper (Optimized for performance)
        public async Task<IEnumerable<ItemCodeIndexViewModel>> GetItemCodeIndexAsync(
            string? code, string? itemName, 
            string? purchasesUnitRate, string? purchasesXeroAccount, string? salesUnitRate, string? salesXeroAccount,
            string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Item Codes list using Dapper.");
            try
            {
                var query = "EXEC dbo.ItemCodeGetIndexView @Code, @ItemName, @PurchasesUnitRate, @PurchasesXeroAccount, @SalesUnitRate, @SalesXeroAccount, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Code = code,
                    ItemName = itemName,
                    PurchasesUnitRate = purchasesUnitRate,
                    PurchasesXeroAccount = purchasesXeroAccount,
                    SalesUnitRate = salesUnitRate,
                    SalesXeroAccount = salesXeroAccount,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<ItemCodeIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Item codes from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<ItemCodeViewModel>> FindDuplicateItemCodeAsync(string code, string itemName, int? itemCodeId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM ItemCodes WHERE IsDeleted = 0 AND (Code = @code OR ItemName = @itemName)";

            if (itemCodeId.HasValue)
            {
                query += " AND itemCodeId != @itemCodeId"; // Exclude a specific ItemCode (useful when updating)
            }

            var parameters = new { Code = code, ItemName = itemName, itemCodeId = itemCodeId };

            return await _unitOfWork.DapperRepository.QueryAsync<ItemCodeViewModel>(query, parameters);
        }

        public async Task<ItemCodeViewModel?> GetItemCodeViewModelByIdAsync(int itemCodeId)
        {
            var ItemCode = await _unitOfWork.ItemCodeRepository.GetItemCodeByIdAsync(itemCodeId);

            if (ItemCode == null)
            {
                return null; // No match found
            }

            // Convert `ItemCode` entity to `ItemCodeViewModel`
            return new ItemCodeViewModel
            {
                itemCodeId = ItemCode.itemCodeId,
                Code = ItemCode.Code,
                ItemName = ItemCode.ItemName,
                ItemDescription = ItemCode.ItemDescription,
                PurchasesDescription = ItemCode.PurchasesDescription,
                SalesDescription = ItemCode.SalesDescription,
                PurchasesUnitRate = ItemCode.PurchasesUnitRate,
                SalesUnitRate = ItemCode.SalesUnitRate,
                PurchasesXeroAccount = ItemCode.PurchasesXeroAccount,
                SalesXeroAccount = ItemCode.SalesXeroAccount,
                IsActive = ItemCode.IsActive
            };
        }

        public async Task<ItemCodeViewModel?> GetItemCodeViewModelWithItemCodeRatesByIdAsync(int itemCodeId)
        {
            var ItemCode = await _unitOfWork.ItemCodeRepository.GetItemCodeWithItemCodeRatesByIdAsync(itemCodeId);

            if (ItemCode == null)
            {
                return null; // No match found
            }

            // Convert `ItemCode` entity to `ItemCodeViewModel`
            return new ItemCodeViewModel
            {
                itemCodeId = ItemCode.itemCodeId,
                Code = ItemCode.Code,
                ItemName = ItemCode.ItemName,
                ItemDescription = ItemCode.ItemDescription,
                PurchasesDescription = ItemCode.PurchasesDescription,
                SalesDescription = ItemCode.SalesDescription,
                PurchasesUnitRate = ItemCode.PurchasesUnitRate,
                SalesUnitRate = ItemCode.SalesUnitRate,
                PurchasesXeroAccount = ItemCode.PurchasesXeroAccount,
                SalesXeroAccount = ItemCode.SalesXeroAccount,
                IsActive = ItemCode.IsActive,

                //ItemCodeRatesList = ItemCode.AssetTypes
                //    .OrderBy(a => a.Name)
                //    .Select(a => new AssetTypeIndexViewModel
                //    {
                //        assetTypeId = a.assetTypeId,
                //        Name = a.Name,
                //        IsActive = a.IsActive
                //    })
                //.ToList()
            };
        }

        // Creates a new ItemCode (EF Core)
        public async Task<int> AddItemCodeAsync(ItemCodeViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new Item code: {model.Code}");

            var itemCode = new ItemCode
            {
                Code = model.Code,
                ItemName = model.ItemName,
                ItemDescription = model.ItemDescription,
                PurchasesDescription = model.PurchasesDescription,
                SalesDescription = model.SalesDescription,
                PurchasesUnitRate = model.PurchasesUnitRate,
                SalesUnitRate = model.SalesUnitRate,
                PurchasesXeroAccount = model.PurchasesXeroAccount,
                SalesXeroAccount = model.SalesXeroAccount,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.ItemCodeRepository.AddItemCodeAsync(itemCode);

            return itemCode.itemCodeId;
        }

        // Updates an existing ItemCode (EF Core)
        public async Task UpdateItemCodeAsync(ItemCodeViewModel model, string updatedById)
        {
            var itemCode = await _unitOfWork.ItemCodeRepository.GetItemCodeByIdAsync(model.itemCodeId);
            if (itemCode == null)
            {
                _logger.LogWarning($"Item code with ID {model.itemCodeId} not found.");
                throw new Exception("Item code not found.");
            }

            itemCode.Code = model.Code;
            itemCode.ItemName = model.ItemName;
            itemCode.ItemDescription = model.ItemDescription;
            itemCode.PurchasesDescription = model.PurchasesDescription;
            itemCode.SalesDescription = model.SalesDescription;
            itemCode.PurchasesUnitRate = model.PurchasesUnitRate;
            itemCode.SalesUnitRate = model.SalesUnitRate;
            itemCode.PurchasesXeroAccount = model.PurchasesXeroAccount;
            itemCode.SalesXeroAccount = model.SalesXeroAccount;
            itemCode.IsActive = model.IsActive;
            itemCode.UpdatedAt = DateTime.UtcNow;
            itemCode.UpdatedById = updatedById;

            await _unitOfWork.ItemCodeRepository.UpdateItemCodeAsync(itemCode);
        }

        public async Task<bool> SoftDeleteItemCodeAsync(int itemCodeId, string updatedById)
        {
            var ItemCode = await _unitOfWork.ItemCodeRepository.GetItemCodeByIdAsync(itemCodeId);

            if (ItemCode == null)
            {
                throw new KeyNotFoundException("Item code not found.");
            }

            // Soft delete staff
            ItemCode.IsDeleted = true;
            ItemCode.IsActive = false;
            ItemCode.UpdatedAt = DateTime.UtcNow;
            ItemCode.UpdatedById = updatedById;

            await _unitOfWork.ItemCodeRepository.UpdateItemCodeAsync(ItemCode);

            return true;
        }
    }
}
