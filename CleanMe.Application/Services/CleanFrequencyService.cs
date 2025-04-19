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
    public class CleanFrequencyService : ICleanFrequencyService
    {
        private readonly IRepository<CleanFrequency> _efCoreRepository; // For EF Core CRUD
        private readonly ICleanFrequencyRepository _CleanFrequencyRepository; // For Dapper queries
        private readonly IDapperRepository _dapperRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StaffService> _logger;

        public CleanFrequencyService(
            IRepository<CleanFrequency> efCoreRepository,
            ICleanFrequencyRepository CleanFrequencyRepository,
            IDapperRepository dapperRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            ILogger<StaffService> logger)
        {
            _efCoreRepository = efCoreRepository;
            _CleanFrequencyRepository = CleanFrequencyRepository;
            _dapperRepository = dapperRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Retrieve a list of CleanFrequencys using Dapper (Optimized for performance)
        public async Task<IEnumerable<CleanFrequencyIndexViewModel>> GetCleanFrequencyIndexAsync(
            string? name, string? description, string? code, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Clean Frequencies list using Dapper.");
            return await _CleanFrequencyRepository.GetCleanFrequencyIndexAsync(
                name, description, code, isActive,
                sortColumn, sortOrder, pageNumber, pageSize);
        }

        public async Task<IEnumerable<CleanFrequencyViewModel>> FindDuplicateCleanFrequencyAsync(string name, string code, int? cleanFrequencyId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM CleanFrequencies WHERE IsDeleted = 0 AND (Name = @name OR Code = @code)";

            if (cleanFrequencyId.HasValue)
            {
                query += " AND CleanFrequencyId != @CleanFrequencyId"; // Exclude a specific CleanFrequency (useful when updating)
            }

            var parameters = new { Name = name, Code = code, CleanFrequencyId = cleanFrequencyId };

            return await _dapperRepository.QueryAsync<CleanFrequencyViewModel>(query, parameters);
        }

        public async Task<CleanFrequencyViewModel?> GetCleanFrequencyByIdAsync(int cleanFrequencyId)
        {
            var CleanFrequency = await _CleanFrequencyRepository.GetCleanFrequencyByIdAsync(cleanFrequencyId);

            if (CleanFrequency == null)
            {
                return null; // No match found
            }

            // Convert `CleanFrequency` entity to `CleanFrequencyViewModel`
            return new CleanFrequencyViewModel
            {
                cleanFrequencyId = CleanFrequency.cleanFrequencyId,
                Name = CleanFrequency.Name,
                Description = CleanFrequency.Description,
                Code = CleanFrequency.Code,
                IsActive = CleanFrequency.IsActive
            };
        }

        // Creates a new CleanFrequency (EF Core)
        public async Task<int> AddCleanFrequencyAsync(CleanFrequencyViewModel model, string addedById)
        {
            _logger.LogInformation($"Adding new cleaning frequency: {model.Name}");

            var CleanFrequency = new CleanFrequency
            {
                Name = model.Name,
                Description = model.Description,
                Code = model.Code,
                IsActive = model.IsActive,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _efCoreRepository.AddAsync(CleanFrequency);
            await _unitOfWork.CommitAsync();

            return CleanFrequency.cleanFrequencyId;
        }

        // Updates an existing CleanFrequency (EF Core)
        public async Task UpdateCleanFrequencyAsync(CleanFrequencyViewModel model, string updatedById)
        {
            var CleanFrequency = await _efCoreRepository.GetByIdAsync(model.cleanFrequencyId);
            if (CleanFrequency == null)
            {
                _logger.LogWarning($"Clean Frequency with ID {model.cleanFrequencyId} not found.");
                throw new Exception("Clean Frequency not found.");
            }

            CleanFrequency.Name = model.Name;
            CleanFrequency.Description = model.Description;
            CleanFrequency.Code = model.Code;
            CleanFrequency.IsActive = model.IsActive;
            CleanFrequency.UpdatedAt = DateTime.UtcNow;
            CleanFrequency.UpdatedById = updatedById;

            _efCoreRepository.Update(CleanFrequency);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteCleanFrequencyAsync(int cleanFrequencyId, string updatedById)
        {
            var CleanFrequency = await _efCoreRepository.GetByIdAsync(cleanFrequencyId);

            if (CleanFrequency == null)
            {
                throw new KeyNotFoundException("Clean Frequency not found.");
            }

            // Soft delete staff
            CleanFrequency.IsDeleted = true;
            CleanFrequency.IsActive = false;
            CleanFrequency.UpdatedAt = DateTime.UtcNow;
            CleanFrequency.UpdatedById = updatedById;

            _efCoreRepository.Update(CleanFrequency);
            await _unitOfWork.CommitAsync();

            _efCoreRepository.Update(CleanFrequency);
            int rowsAffected = await _unitOfWork.CommitAsync(); // Returns the number of affected rows

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }
    }
}
