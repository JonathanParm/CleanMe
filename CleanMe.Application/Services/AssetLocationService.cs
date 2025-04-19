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
    public class AssetLocationService : IAssetLocationService
    {
        private readonly IRepository<AssetLocation> _efCoreRepository; // For EF Core CRUD
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetLocationRepository _assetLocationRepository;
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserService _userService;
        private readonly ILogger<AssetLocationService> _logger;

        public AssetLocationService(
            IRepository<AssetLocation> efCoreRepository,
            IUnitOfWork unitOfWork,
            IAssetLocationRepository AssetLocationRepository,
            IDapperRepository dapperRepository,
            IUserService userService,
            ILogger<AssetLocationService> logger)
        {
            _efCoreRepository = efCoreRepository;
            _unitOfWork = unitOfWork;
            _assetLocationRepository = AssetLocationRepository;
            _dapperRepository = dapperRepository;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of AssetLocation members using Dapper (Optimized for performance)
        public async Task<IEnumerable<AssetLocationIndexViewModel>> GetAssetLocationIndexAsync(
            string? areaName, string? description, string? townSuburb, string? reportCode, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching AssetLocation list using Dapper.");
            return await _assetLocationRepository.GetAssetLocationIndexAsync(
                areaName, description, townSuburb, reportCode, isActive,
                sortColumn, sortOrder, pageNumber, pageSize);
        }

        public async Task<IEnumerable<AssetLocationViewModel>> FindDuplicateAssetLocationAsync(string description, string? reportCode, int? assetLocationId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM AssetLocations WHERE IsDeleted = 0 AND (Description = @Description OR ReportCode = @ReportCode)";

            if (assetLocationId.HasValue)
            {
                query += " AND assetLocationId != @assetLocationId"; // Exclude a specific AssetLocation member (useful when updating)
            }

            var parameters = new { Description = description, ReportCode = reportCode, assetLocationId = assetLocationId };

            var duplicateAssetLocation = await _dapperRepository.QueryAsync<AssetLocationWithAddressDto>(query, parameters);

            // Map `AssetLocation` entity to `AssetLocationViewModel`
            return duplicateAssetLocation.Select(al => new AssetLocationViewModel
            {
                assetLocationId = al.assetLocationId,
                Description = al.Description,
                areaId = al.areaId,
                Address = new AddressViewModel
                {
                    Line1 = al.AddressLine1,
                    Line2 = al.AddressLine2,
                    Suburb = al.AddressSuburb,
                    TownOrCity = al.AddressTownOrCity,
                    Postcode = al.AddressPostcode
                },
                SequenceOrder = al.SequenceOrder,
                SeqNo = al.SeqNo,
                ReportCode = al.ReportCode,
                AccNo = al.AccNo,
                IsActive = al.IsActive
            });
        }

        public async Task<AssetLocationViewModel?> GetAssetLocationViewModelByIdAsync(int assetLocationId)
        {
            var assetLocation = await _assetLocationRepository.GetAssetLocationViewModelByIdAsync(assetLocationId); // Calls repository

            if (assetLocation == null)
            {
                return null; // No match found
            }

            // Convert `AssetLocation` entity to `AssetLocationViewModel`
            return new AssetLocationViewModel
            {
                assetLocationId = assetLocation.assetLocationId,
                Description = assetLocation.Description,
                areaId = assetLocation.areaId,
                Address = new AddressViewModel
                {
                    Line1 = assetLocation.Address.Line1,
                    Line2 = assetLocation.Address.Line2,
                    Suburb = assetLocation.Address.Suburb,
                    TownOrCity = assetLocation.Address.TownOrCity,
                    Postcode = assetLocation.Address.Postcode,
                },
                SequenceOrder = assetLocation.SequenceOrder,
                SeqNo = assetLocation.SeqNo,
                ReportCode = assetLocation.ReportCode,
                AccNo = assetLocation.AccNo,
                IsActive = assetLocation.IsActive
            };
        }
        public async Task<AssetLocationViewModel> PrepareNewAssetLocationViewModelAsync(int areaId)
        {
            return await _assetLocationRepository.PrepareNewAssetLocationViewModelAsync(areaId);
        }

        // Creates a new AssetLocation member (EF Core)
        public async Task<int> AddAssetLocationAsync(AssetLocationViewModel model, string addedById)
        {
            _logger.LogInformation($"Creating new AssetLocation : {model.Description}");

            var AssetLocation = new AssetLocation
            {
                Description = model.Description,
                areaId = model.areaId,
                Address = new Address()
                {
                    Line1 = model.Address.Line1,
                    Line2 = model.Address.Line2,
                    Suburb = model.Address.Suburb,
                    TownOrCity = model.Address.TownOrCity,
                    Postcode = model.Address.Postcode,
                },
                SequenceOrder = model.SequenceOrder,
                SeqNo = model.SeqNo,
                ReportCode = model.ReportCode,
                AccNo = model.AccNo,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _efCoreRepository.AddAsync(AssetLocation);
            await _unitOfWork.CommitAsync();

            return AssetLocation.assetLocationId;
        }

        // Updates an existing AssetLocation member (EF Core)
        public async Task UpdateAssetLocationAsync(AssetLocationViewModel model, string updatedById)
        {
            var assetLocation = await _efCoreRepository.GetByIdAsync(model.assetLocationId);
            if (assetLocation == null)
            {
                _logger.LogWarning($"AssetLocation member with ID {model.assetLocationId} not found.");
                throw new Exception("AssetLocation member not found.");
            }

            assetLocation.Description = model.Description;
            assetLocation.areaId = model.areaId;
            assetLocation.Address = new Address()
            {
                Line1 = model.Address.Line1,
                Line2 = model.Address.Line2,
                Suburb = model.Address.Suburb,
                TownOrCity = model.Address.TownOrCity,
                Postcode = model.Address.Postcode,
            };
            assetLocation.SequenceOrder = model.SequenceOrder;
            assetLocation.SeqNo = model.SeqNo;
            assetLocation.ReportCode = model.ReportCode;
            assetLocation.AccNo = model.AccNo;
            assetLocation.IsActive = model.IsActive;
            assetLocation.UpdatedAt = DateTime.UtcNow;
            assetLocation.UpdatedById = updatedById;

            _efCoreRepository.Update(assetLocation);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteAssetLocationAsync(int assetLocationId, string updatedById)
        {
            var assetLocation = await _efCoreRepository.GetByIdAsync(assetLocationId);

            if (assetLocation == null)
            {
                throw new KeyNotFoundException("AssetLocation member not found.");
            }

            // Soft delete AssetLocation
            assetLocation.IsDeleted = true;
            assetLocation.IsActive = false;
            assetLocation.UpdatedAt = DateTime.UtcNow;
            assetLocation.UpdatedById = updatedById;

            _efCoreRepository.Update(assetLocation);
            await _unitOfWork.CommitAsync();

            _efCoreRepository.Update(assetLocation);
            int rowsAffected = await _unitOfWork.CommitAsync(); // Returns the number of affected rows

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }

        // Assigns a login account to a AssetLocation member (via UserService)
        public async Task<bool> AssignApplicationUserAsync(int assetLocationId, string email, string password)
        {
            var assetLocation = await _efCoreRepository.GetByIdAsync(assetLocationId);
            if (assetLocation == null)
            {
                _logger.LogWarning($"AssetLocation with ID {assetLocationId} not found for user assignment.");
                return false;
            }

            _efCoreRepository.Update(assetLocation);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task UpdateAssetLocationApplicationUserId(int assetLocationId, string applicationUserId)
        {
            var assetLocation = await _efCoreRepository.GetByIdAsync(assetLocationId);

            if (assetLocation == null)
            {
                throw new KeyNotFoundException($"AssetLocation member with ID {assetLocationId} not found.");
            }

            _efCoreRepository.Update(assetLocation);
            await _unitOfWork.CommitAsync();
        }

    }
}
