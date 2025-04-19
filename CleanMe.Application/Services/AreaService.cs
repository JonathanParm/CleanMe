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
    public class AreaService : IAreaService
    {
        private readonly IRepository<Area> _efCoreRepository; // For EF Core CRUD
        private readonly IAreaRepository _areaRepository; // For Dapper queries
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StaffService> _logger;

        public AreaService(
            IRepository<Area> efCoreRepository,
            IAreaRepository areaRepository,
            IDapperRepository dapperRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            ILogger<StaffService> logger)
        {
            _efCoreRepository = efCoreRepository;
            _areaRepository = areaRepository;
            _dapperRepository = dapperRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Retrieve a list of Areas using Dapper (Optimized for performance)
        public async Task<IEnumerable<AreaIndexViewModel>> GetAreaIndexAsync(
            string? regionName, string? name, int? reportCode, string? isActive,
                string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Areas list using Dapper.");
            return await _areaRepository.GetAreaIndexAsync(
                regionName, name, reportCode, isActive,
                sortColumn, sortOrder, pageNumber, pageSize);
        }

        public async Task<IEnumerable<AreaViewModel>> FindDuplicateAreaAsync(string name, int reportCode, int? areaId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Areas WHERE IsDeleted = 0 AND (Name = @name OR ReportCode = @reportCode)";

            if (areaId.HasValue)
            {
                query += " AND areaId != @areaId"; // Exclude a specific Area (useful when updating)
            }

            var parameters = new { Name = name, ReportCode = reportCode, areaId = areaId };

            return await _dapperRepository.QueryAsync<AreaViewModel>(query, parameters);
        }

        public async Task<AreaViewModel?> GetAreaByIdAsync(int areaId)
        {
            return await _areaRepository.GetAreaByIdAsync(areaId);
        }

        public async Task<AreaViewModel?> GetAreaViewModelByIdAsync(int areaId)
        {
            return await _areaRepository.GetAreaViewModelByIdAsync(areaId);
        }

        public async Task<AreaViewModel?> GetAreaViewModelWithAssetLocationsByIdAsync(int areaId)
        {
            var area = await _areaRepository.GetAreaViewModelWithAssetLocationsByIdAsync(areaId);

            if (area == null)
            {
                return null; // No match found
            }

            // Convert `Area` entity to `AreaViewModel`
            return new AreaViewModel
            {
                areaId = area.areaId,
                Name = area.Name,
                regionId = area.regionId,
                RegionName = area.Region?.Name,
                SequenceOrder = area.SequenceOrder,
                SeqNo = area.SeqNo,
                ReportCode = area.ReportCode,
                IsActive = area.IsActive,
                AssetLocationsList = area.AssetLocations
                .OrderBy(al => al.SequenceOrder)
                .Select(al => new AssetLocationIndexViewModel
                {
                    assetLocationId = al.assetLocationId,
                    Description = al.Description,
                    ReportCode = al.ReportCode,
                    TownSuburb = string.Join(", ", new[] { al.Address.TownOrCity, al.Address.Suburb }
    .Where(s => !string.IsNullOrWhiteSpace(s))),
                    IsActive = al.IsActive
                })
                .ToList()
            };
        }

        public async Task<AreaViewModel> PrepareNewAreaViewModelAsync(int regionId)
        {
            return await _areaRepository.PrepareNewAreaViewModelAsync(regionId);
        }

        // Creates a new Area (EF Core)
        public async Task<int> AddAreaAsync(AreaViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new area: {model.Name}");

            var Area = new Area
            {
                Name = model.Name,
                regionId = model.regionId,
                ReportCode = model.ReportCode,
                SequenceOrder = model.SequenceOrder,
                SeqNo = model.SeqNo,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _efCoreRepository.AddAsync(Area);
            await _unitOfWork.CommitAsync();

            return Area.areaId;
        }

        // Updates an existing Area (EF Core)
        public async Task UpdateAreaAsync(AreaViewModel model, string updatedById)
        {
            var Area = await _efCoreRepository.GetByIdAsync(model.areaId);
            if (Area == null)
            {
                _logger.LogWarning($"Area with ID {model.areaId} not found.");
                throw new Exception("Area not found.");
            }

            Area.Name = model.Name;
            Area.regionId = model.regionId;
            Area.ReportCode = model.ReportCode;
            Area.SequenceOrder = model.SequenceOrder;
            Area.SeqNo = model.SeqNo;
            Area.IsActive = model.IsActive;
            Area.UpdatedAt = DateTime.UtcNow;
            Area.UpdatedById = updatedById;

            _efCoreRepository.Update(Area);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteAreaAsync(int areaId, string updatedById)
        {
            var Area = await _efCoreRepository.GetByIdAsync(areaId);

            if (Area == null)
            {
                throw new KeyNotFoundException("Area not found.");
            }

            // Soft delete staff
            Area.IsDeleted = true;
            Area.IsActive = false;
            Area.UpdatedAt = DateTime.UtcNow;
            Area.UpdatedById = updatedById;

            _efCoreRepository.Update(Area);
            await _unitOfWork.CommitAsync();

            _efCoreRepository.Update(Area);
            int rowsAffected = await _unitOfWork.CommitAsync(); // Returns the number of affected rows

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }
    }
}
