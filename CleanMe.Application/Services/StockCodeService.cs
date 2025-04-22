using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanMe.Application.Services
{
    public class StockCodeService : IStockCodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public StockCodeService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of StockCodes using Dapper (Optimized for performance)
        public async Task<IEnumerable<StockCodeIndexViewModel>> GetStockCodeIndexAsync(
            string? name, string? description, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Stock Codes list using Dapper.");
            try
            {
                var query = "EXEC dbo.StockCodeGetIndexView @Name, @Description, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
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

                return await _unitOfWork.DapperRepository.QueryAsync<StockCodeIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching Stock Codes from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<StockCodeViewModel>> FindDuplicateStockCodeAsync(string name, int? stockCodeId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM StockCodes WHERE IsDeleted = 0 AND Name = @name";

            if (stockCodeId.HasValue)
            {
                query += " AND stockCodeId != @stockCodeId"; // Exclude a specific StockCode (useful when updating)
            }

            var parameters = new { Name = name, stockCodeId = stockCodeId };

            return await _unitOfWork.DapperRepository.QueryAsync<StockCodeViewModel>(query, parameters);
        }

        public async Task<StockCodeViewModel?> GetStockCodeViewModelByIdAsync(int stockCodeId)
        {
            var StockCode = await _unitOfWork.StockCodeRepository.GetStockCodeByIdAsync(stockCodeId);

            if (StockCode == null)
            {
                return null; // No match found
            }

            // Convert `StockCode` entity to `StockCodeViewModel`
            return new StockCodeViewModel
            {
                stockCodeId = StockCode.stockCodeId,
                Name = StockCode.Name,
                Description = StockCode.Description,
                IsActive = StockCode.IsActive
            };
        }

        public async Task<StockCodeViewModel?> GetStockCodeViewModelWithAssetTypesByIdAsync(int stockCodeId)
        {
            var StockCode = await _unitOfWork.StockCodeRepository.GetStockCodeWithAssetTypesByIdAsync(stockCodeId);

            if (StockCode == null)
            {
                return null; // No match found
            }

            // Convert `StockCode` entity to `StockCodeViewModel`
            return new StockCodeViewModel
            {
                stockCodeId = StockCode.stockCodeId,
                Name = StockCode.Name,
                Description = StockCode.Description,
                IsActive = StockCode.IsActive,

                AssetTypesList = StockCode.AssetTypes
                    .OrderBy(a => a.Name)
                    .Select(a => new AssetTypeIndexViewModel
                    {
                        assetTypeId = a.assetTypeId,
                        Name = a.Name,
                        IsActive = a.IsActive
                    })
                .ToList()
            };
        }

        // Creates a new StockCode (EF Core)
        public async Task<int> AddStockCodeAsync(StockCodeViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new stock code: {model.Name}");

            var StockCode = new StockCode
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.StockCodeRepository.AddStockCodeAsync(StockCode);

            return StockCode.stockCodeId;
        }

        // Updates an existing StockCode (EF Core)
        public async Task UpdateStockCodeAsync(StockCodeViewModel model, string updatedById)
        {
            var StockCode = await _unitOfWork.StockCodeRepository.GetStockCodeByIdAsync(model.stockCodeId);
            if (StockCode == null)
            {
                _logger.LogWarning($"Stock Code with ID {model.stockCodeId} not found.");
                throw new Exception("Stock Code not found.");
            }

            StockCode.Name = model.Name;
            StockCode.Description = model.Description;
            StockCode.IsActive = model.IsActive;
            StockCode.UpdatedAt = DateTime.UtcNow;
            StockCode.UpdatedById = updatedById;

            await _unitOfWork.StockCodeRepository.UpdateStockCodeAsync(StockCode);
        }

        public async Task<bool> SoftDeleteStockCodeAsync(int stockCodeId, string updatedById)
        {
            var StockCode = await _unitOfWork.StockCodeRepository.GetStockCodeByIdAsync(stockCodeId);

            if (StockCode == null)
            {
                throw new KeyNotFoundException("Stock Code not found.");
            }

            // Soft delete staff
            StockCode.IsDeleted = true;
            StockCode.IsActive = false;
            StockCode.UpdatedAt = DateTime.UtcNow;
            StockCode.UpdatedById = updatedById;

            await _unitOfWork.StockCodeRepository.UpdateStockCodeAsync(StockCode);

            return true;
        }
    }
}
