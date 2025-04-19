using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Interfaces;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using CleanMe.Domain.Common;
using CleanMe.Application.DTOs;

namespace CleanMe.Application.Services
{
    public class AssetTypeService : IAssetTypeService
    {
        private readonly IRepository<AssetType> _efCoreRepository; // For EF Core CRUD
        private readonly IAssetTypeRepository _assetTypeRepository; // For Dapper queries
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StaffService> _logger;

        public AssetTypeService(
            IRepository<AssetType> efCoreRepository,
            IAssetTypeRepository assetTypeRepository,
            IDapperRepository dapperRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            ILogger<StaffService> logger)
        {
            _efCoreRepository = efCoreRepository;
            _assetTypeRepository = assetTypeRepository;
            _dapperRepository = dapperRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Retrieve a list of AssetTypes using Dapper (Optimized for performance)
        public async Task<IEnumerable<AssetTypeIndexViewModel>> GetAssetTypeIndexAsync(
            string? name, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Asset Types list using Dapper.");
            return await _assetTypeRepository.GetAssetTypeIndexAsync(
                name, isActive,
                sortColumn, sortOrder, pageNumber, pageSize);
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

            return await _dapperRepository.QueryAsync<AssetTypeViewModel>(query, parameters);
        }

        public async Task<AssetTypeViewModel?> GetAssetTypeByIdAsync(int assetTypeId)
        {
            var assetType = await _assetTypeRepository.GetAssetTypeByIdAsync(assetTypeId);

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

            await _efCoreRepository.AddAsync(AssetType);
            await _unitOfWork.CommitAsync();

            return AssetType.assetTypeId;
        }

        // Updates an existing AssetType (EF Core)
        public async Task UpdateAssetTypeAsync(AssetTypeViewModel model, string updatedById)
        {
            var AssetType = await _efCoreRepository.GetByIdAsync(model.assetTypeId);
            if (AssetType == null)
            {
                _logger.LogWarning($"Asset Type with ID {model.assetTypeId} not found.");
                throw new Exception("Asset Type not found.");
            }

            AssetType.Name = model.Name;
            AssetType.IsActive = model.IsActive;
            AssetType.UpdatedAt = DateTime.UtcNow;
            AssetType.UpdatedById = updatedById;

            _efCoreRepository.Update(AssetType);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteAssetTypeAsync(int assetTypeId, string updatedById)
        {
            var AssetType = await _efCoreRepository.GetByIdAsync(assetTypeId);

            if (AssetType == null)
            {
                throw new KeyNotFoundException("Asset Type not found.");
            }

            // Soft delete staff
            AssetType.IsDeleted = true;
            AssetType.IsActive = false;
            AssetType.UpdatedAt = DateTime.UtcNow;
            AssetType.UpdatedById = updatedById;

            _efCoreRepository.Update(AssetType);
            await _unitOfWork.CommitAsync();

            _efCoreRepository.Update(AssetType);
            int rowsAffected = await _unitOfWork.CommitAsync(); // Returns the number of affected rows

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }
    }
}
