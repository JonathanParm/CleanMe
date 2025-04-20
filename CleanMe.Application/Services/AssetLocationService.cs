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
using System.Collections;
using System.Data;

namespace CleanMe.Application.Services
{
    public class AssetLocationService : IAssetLocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserService _userService;
        private readonly ILogger<AssetLocationService> _logger;

        public AssetLocationService(
            IUnitOfWork unitOfWork,

            IUserService userService,
            ILogger<AssetLocationService> logger)
        {
            _unitOfWork = unitOfWork;

            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of AssetLocation members using Dapper (Optimized for performance)
        public async Task<IEnumerable<AssetLocationIndexViewModel>> GetAssetLocationIndexAsync(
            string? areaName, string? description, string? townSuburb, string? reportCode, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching AssetLocation list using Dapper.");
            try
            {
                var query = "EXEC dbo.AssetLocationGetIndexView @AreaName, @Description, @TownSuburb, @ReportCode, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    AreaName = areaName,
                    Description = description,
                    TownSuburb = townSuburb,
                    ReportCode = reportCode,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<AssetLocationIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching AssetLocations from stored procedure", ex);
            }
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

            var duplicateAssetLocation = await _unitOfWork.DapperRepository.QueryAsync<AssetLocationWithAddressDto>(query, parameters);

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
            var query = "dbo.AssetLocationGetById";
            var parameters = new { assetLocationId = assetLocationId };

            var results = await _unitOfWork.DapperRepository.QueryAsync<AssetLocationWithAddressDto>(query, parameters, CommandType.StoredProcedure);
            var row = results.FirstOrDefault();
            if (row == null) return null;

            // Convert `AssetLocation` entity to `AssetLocationViewModel`
            return new AssetLocationViewModel
            {
                assetLocationId = row.assetLocationId,
                Description = row.Description,
                areaId = row.areaId,
                Address = new AddressViewModel
                {
                    Line1 = row.AddressLine1,
                    Line2 = row.AddressLine2,
                    Suburb = row.AddressSuburb,
                    TownOrCity = row.AddressTownOrCity,
                    Postcode = row.AddressPostcode
                },
                SequenceOrder = row.SequenceOrder,
                SeqNo = row.SeqNo,
                ReportCode = row.ReportCode,
                AccNo = row.AccNo,
                IsActive = row.IsActive
            };
        }
        public async Task<AssetLocationViewModel> PrepareNewAssetLocationViewModelAsync(int areaId)
        {
            var area = await _unitOfWork.AreaRepository.GetAreaByIdAsync(areaId);

            if (area == null)
                throw new Exception("Area not found.");

            return new AssetLocationViewModel
            {
                areaId = area.areaId,
                AreaName = area.Name,
                IsActive = true
            };
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

            await _unitOfWork.AssetLocationRepository.AddAssetLocationAsync(AssetLocation);

            return AssetLocation.assetLocationId;
        }

        public async Task UpdateAssetLocationAsync(AssetLocationViewModel model, string updatedById)
        {
            var assetLocation = await _unitOfWork.AssetLocationRepository.GetAssetLocationByIdAsync(model.assetLocationId);
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

            await _unitOfWork.AssetLocationRepository.UpdateAssetLocationAsync(assetLocation);
        }

        public async Task<bool> SoftDeleteAssetLocationAsync(int assetLocationId, string updatedById)
        {
            var assetLocation = await _unitOfWork.AssetLocationRepository.GetAssetLocationByIdAsync(assetLocationId);

            if (assetLocation == null)
            {
                throw new KeyNotFoundException("AssetLocation member not found.");
            }

            // Soft delete AssetLocation
            assetLocation.IsDeleted = true;
            assetLocation.IsActive = false;
            assetLocation.UpdatedAt = DateTime.UtcNow;
            assetLocation.UpdatedById = updatedById;

            await _unitOfWork.AssetLocationRepository.UpdateAssetLocationAsync(assetLocation);

            return true;
        }

        // Assigns a login account to a AssetLocation member (via UserService)
        public async Task<bool> AssignApplicationUserAsync(int assetLocationId, string email, string password)
        {
            var assetLocation = await _unitOfWork.AssetLocationRepository.GetAssetLocationByIdAsync(assetLocationId);
            if (assetLocation == null)
            {
                _logger.LogWarning($"AssetLocation with ID {assetLocationId} not found for user assignment.");
                return false;
            }

            await _unitOfWork.AssetLocationRepository.UpdateAssetLocationAsync(assetLocation);
            return true;
        }

        public async Task UpdateAssetLocationApplicationUserId(int assetLocationId, string applicationUserId)
        {
            var assetLocation = await _unitOfWork.AssetLocationRepository.GetAssetLocationByIdAsync(assetLocationId);

            if (assetLocation == null)
            {
                throw new KeyNotFoundException($"AssetLocation member with ID {assetLocationId} not found.");
            }

            await _unitOfWork.AssetLocationRepository.UpdateAssetLocationAsync(assetLocation);
        }

    }
}
