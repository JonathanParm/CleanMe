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

namespace CleanMe.Application.Services
{
    public class CleanFrequencyService : ICleanFrequencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public CleanFrequencyService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of CleanFrequencys using Dapper (Optimized for performance)
        public async Task<IEnumerable<CleanFrequencyIndexViewModel>> GetCleanFrequencyIndexAsync(
            string? name, string? description, string? code, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching Clean Frequencies list using Dapper.");
            try
            {
                var query = "EXEC dbo.CleanFrequencyGetIndexView @Name, @Description, @Code, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";
                var parameters = new
                {
                    Name = name,
                    Description = description,
                    Code = code,
                    IsActive = isActive,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return await _unitOfWork.DapperRepository.QueryAsync<CleanFrequencyIndexViewModel>(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error fetching CleanFrequencys from stored procedure", ex);
            }
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

            return await _unitOfWork.DapperRepository.QueryAsync<CleanFrequencyViewModel>(query, parameters);
        }

        public async Task<CleanFrequencyViewModel?> GetCleanFrequencyViewModelByIdAsync(int cleanFrequencyId)
        {
            var CleanFrequency = await _unitOfWork.CleanFrequencyRepository.GetCleanFrequencyByIdAsync(cleanFrequencyId);

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

            var cleanFrequency = new CleanFrequency
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

            await _unitOfWork.CleanFrequencyRepository.AddCleanFrequencyAsync(cleanFrequency);

            return cleanFrequency.cleanFrequencyId;
        }

        // Updates an existing CleanFrequency (EF Core)
        public async Task UpdateCleanFrequencyAsync(CleanFrequencyViewModel model, string updatedById)
        {
            var cleanFrequency = await _unitOfWork.CleanFrequencyRepository.GetCleanFrequencyByIdAsync(model.cleanFrequencyId);
            if (cleanFrequency == null)
            {
                _logger.LogWarning($"Clean Frequency with ID {model.cleanFrequencyId} not found.");
                throw new Exception("Clean Frequency not found.");
            }

            cleanFrequency.Name = model.Name;
            cleanFrequency.Description = model.Description;
            cleanFrequency.Code = model.Code;
            cleanFrequency.IsActive = model.IsActive;
            cleanFrequency.UpdatedAt = DateTime.UtcNow;
            cleanFrequency.UpdatedById = updatedById;

            await _unitOfWork.CleanFrequencyRepository.UpdateCleanFrequencyAsync(cleanFrequency);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteCleanFrequencyAsync(int cleanFrequencyId, string updatedById)
        {
            var cleanFrequency = await _unitOfWork.CleanFrequencyRepository.GetCleanFrequencyByIdAsync(cleanFrequencyId);

            if (cleanFrequency == null)
            {
                throw new KeyNotFoundException("Clean Frequency not found.");
            }

            // Soft delete staff
            cleanFrequency.IsDeleted = true;
            cleanFrequency.IsActive = false;
            cleanFrequency.UpdatedAt = DateTime.UtcNow;
            cleanFrequency.UpdatedById = updatedById;

            await _unitOfWork.CleanFrequencyRepository.UpdateCleanFrequencyAsync(cleanFrequency);

            return true;
        }
    }
}
