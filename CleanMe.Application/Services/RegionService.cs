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
    public class RegionService : IRegionService
    {
        private readonly IRepository<Region> _efCoreRepository; // For EF Core CRUD
        private readonly IRegionRepository _regionRepository; // For Dapper queries
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StaffService> _logger;

        public RegionService(
            IRepository<Region> efCoreRepository,
            IRegionRepository regionRepository,
            IDapperRepository dapperRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            ILogger<StaffService> logger)
        {
            _efCoreRepository = efCoreRepository;
            _regionRepository = regionRepository;
            _dapperRepository = dapperRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Retrieve a list of regions using Dapper (Optimized for performance)
        public async Task<IEnumerable<RegionIndexViewModel>> GetRegionIndexAsync(
            string? name, string? code, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching regions list using Dapper.");
            return await _regionRepository.GetRegionIndexAsync(
                name, code, isActive,
                sortColumn, sortOrder, pageNumber, pageSize);
        }

        public async Task<IEnumerable<RegionViewModel>> FindDuplicateRegionAsync(string name, string reportCode, int? regionId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Regions WHERE IsDeleted = 0 AND (Name = @name OR ReportCode = @reportCode)";

            if (regionId.HasValue)
            {
                query += " AND regionId != @regionId"; // Exclude a specific region (useful when updating)
            }

            var parameters = new { Name = name, ReportCode = reportCode, regionId = regionId };

            return await _dapperRepository.QueryAsync<RegionViewModel>(query, parameters);
        }

        public async Task<RegionViewModel?> GetRegionByIdAsync(int regionId)
        {
            var region = await _regionRepository.GetRegionByIdAsync(regionId);

            if (region == null)
            {
                return null; // No match found
            }

            // Convert `Region` entity to `RegionViewModel`
            return new RegionViewModel
            {
                regionId = region.regionId,
                Name = region.Name,
                ReportCode = region.ReportCode,
                IsActive = region.IsActive
            };
        }

        public async Task<RegionViewModel?> GetRegionWithAreasByIdAsync(int regionId)
        {
            var region = await _regionRepository.GetRegionWithAreasByIdAsync(regionId);

            if (region == null)
            {
                return null; // No match found
            }

            // Convert `Region` entity to `RegionViewModel`
            return new RegionViewModel
            {
                regionId = region.regionId,
                Name = region.Name,
                ReportCode = region.ReportCode,
                IsActive = region.IsActive,

                AreasList = region.Areas
                    .OrderBy(a => a.SequenceOrder)
                    .Select(a => new AreaIndexViewModel
                    {
                        areaId = a.areaId,
                        Name = a.Name,
                        RegionName = region.Name,
                        ReportCode = a.ReportCode,
                        IsActive = a.IsActive
                    })
                .ToList()
            };
        }

        // Creates a new region (EF Core)
        public async Task<int> AddRegionAsync(RegionViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new regon: {model.Name}");

            var region = new Region
            {
                Name = model.Name,
                ReportCode = model.ReportCode,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _efCoreRepository.AddAsync(region);
            await _unitOfWork.CommitAsync();

            return region.regionId;
        }

        // Updates an existing region (EF Core)
        public async Task UpdateRegionAsync(RegionViewModel model, string updatedById)
        {
            var region = await _efCoreRepository.GetByIdAsync(model.regionId);
            if (region == null)
            {
                _logger.LogWarning($"Region with ID {model.regionId} not found.");
                throw new Exception("Region not found.");
            }

            region.Name = model.Name;
            region.ReportCode = model.ReportCode;
            region.IsActive = model.IsActive;
            region.UpdatedAt = DateTime.UtcNow;
            region.UpdatedById = updatedById;

            _efCoreRepository.Update(region);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteRegionAsync(int regionId, string updatedById)
        {
            var region = await _efCoreRepository.GetByIdAsync(regionId);

            if (region == null)
            {
                throw new KeyNotFoundException("region not found.");
            }

            // Soft delete staff
            region.IsDeleted = true;
            region.IsActive = false;
            region.UpdatedAt = DateTime.UtcNow;
            region.UpdatedById = updatedById;

            _efCoreRepository.Update(region);
            await _unitOfWork.CommitAsync();

            _efCoreRepository.Update(region);
            int rowsAffected = await _unitOfWork.CommitAsync(); // Returns the number of affected rows

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }
    }
}
