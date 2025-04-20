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
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<StaffService> _logger;

        public StaffService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            ILogger<StaffService> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        // Retrieve a list of staff members using Dapper (Optimized for performance)
        public async Task<IEnumerable<StaffIndexViewModel>> GetStaffIndexAsync(
            string? staffNo, string? fullName, string? workRole, string? contactDetail, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching staff list using Dapper.");
            var query = "EXEC dbo.StaffGetIndexView @StaffNo, @FullName, @WorkRole, @ContactDetail, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";

            var parameters = new
            {
                StaffNo = staffNo,
                FullName = fullName,
                WorkRole = workRole,
                ContactDetail = contactDetail,
                IsActive = isActive,
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return await _unitOfWork.DapperRepository.QueryAsync<StaffIndexViewModel>(query, parameters);
        }

        public async Task<IEnumerable<StaffViewModel>> FindDuplicateStaffAsync(string firstName, string familyName, int? staffNo, int? staffId = null)
        {
            // Exclude any soft deletes
            var query = "SELECT * FROM Staff WHERE IsDeleted = 0 AND ((FirstName = @FirstName AND FamilyName = @FamilyName) OR StaffNo = @staffNo)";

            if (staffId.HasValue)
            {
                query += " AND staffId != @staffId"; // Exclude a specific staff member (useful when updating)
            }

            var parameters = new { FirstName = firstName, FamilyName = familyName, StaffNo = staffNo, staffId = staffId };

            var duplicateStaff = await _unitOfWork.DapperRepository.QueryAsync<StaffWithAddressDto>(query, parameters);

            // Map `Staff` entity to `StaffViewModel`
            return duplicateStaff.Select(s => new StaffViewModel
            {
                StaffId = s.staffId,
                StaffNo = s.StaffNo,
                FirstName = s.FirstName,
                FamilyName = s.FamilyName,
                PhoneHome = s.PhoneHome,
                PhoneMobile = s.PhoneMobile,
                Email = s.Email,
                Address = new AddressViewModel
                {
                    Line1 = s.AddressLine1,
                    Line2 = s.AddressLine2,
                    Suburb = s.AddressSuburb,
                    TownOrCity = s.AddressTownOrCity,
                    Postcode = s.AddressPostcode,
                },
                IrdNumber = s.IrdNumber,
                BankAccount = s.BankAccount,
                PayrollId = s.PayrollId,
                JobTitle = s.JobTitle,
                WorkRole = s.WorkRole,
                IsActive = s.IsActive,
                ApplicationUserId = s.ApplicationUserId
            });
        }

        public async Task<StaffViewModel?> GetStaffViewModelByIdAsync(int staffId)
        {
            var query = "dbo.StaffGetById";
            var parameters = new { staffId = staffId };

            var results = await _unitOfWork.DapperRepository.QueryAsync<StaffWithAddressDto>(query, parameters, CommandType.StoredProcedure);
            var row = results.FirstOrDefault();
            if (row == null) return null;

            // Convert `Staff` entity to `StaffViewModel`
            return new StaffViewModel
            {
                StaffId = row.staffId,
                StaffNo = row.StaffNo,
                FirstName = row.FirstName,
                FamilyName = row.FamilyName,
                PhoneHome = row.PhoneHome,
                PhoneMobile = row.PhoneMobile,
                Email = row.Email,
                Address = new AddressViewModel
                {
                    Line1 = row.AddressLine1,
                    Line2 = row.AddressLine2,
                    Suburb = row.AddressSuburb,
                    TownOrCity = row.AddressTownOrCity,
                    Postcode = row.AddressPostcode
                },
                IrdNumber = row.IrdNumber,
                BankAccount = row.BankAccount,
                PayrollId = row.PayrollId,
                JobTitle = row.JobTitle,
                WorkRole = row.WorkRole,
                IsActive = row.IsActive,
                ApplicationUserId = row.ApplicationUserId
            };
        }

        public async Task<bool> IsEmailAvailableAsync(string email, int? staffId = null)
        {
            var query = "SELECT COUNT(1) FROM Staff WHERE Email = @Email AND IsDeleted = 0";

            if (staffId.HasValue)
            {
                query += " AND StaffId != @StaffId"; // Exclude current record when editing
            }

            var parameters = new { Email = email, StaffId = staffId };

            int count = await _unitOfWork.DapperRepository.ExecuteScalarAsync<int>(query, parameters); // Ensures int return type
            return count == 0; // Email is available if count is 0
        }

        // Creates a new staff member (EF Core)
        public async Task<int> AddStaffAsync(StaffViewModel model, string addedById)
        {
            _logger.LogInformation($"Creating new staff member: {model.Email}");

            var staff = new Staff
            {
                StaffNo = model.StaffNo,
                FirstName = model.FirstName,
                FamilyName = model.FamilyName,
                PhoneHome = model.PhoneHome,
                PhoneMobile = model.PhoneMobile,
                Email = model.Email,
                Address = new Address()
                {
                    Line1 = model.Address.Line1,
                    Line2 = model.Address.Line2,
                    Suburb = model.Address.Suburb,
                    TownOrCity = model.Address.TownOrCity,
                    Postcode = model.Address.Postcode,
                },
                IrdNumber = model.IrdNumber,
                BankAccount = model.BankAccount,
                PayrollId = model.PayrollId,
                JobTitle = model.JobTitle,
                WorkRole = model.WorkRole,
                IsActive = model.IsActive,
                ApplicationUserId = model.ApplicationUserId,
                AddedAt = DateTime.UtcNow,
                AddedById = addedById,
                UpdatedAt = DateTime.UtcNow,
                UpdatedById = addedById
            };

            await _unitOfWork.StaffRepository.AddStaffAsync(staff);

            return staff.staffId;
        }

        // Updates an existing staff member (EF Core)
        public async Task UpdateStaffAsync(StaffViewModel model, string updatedById)
        {
            var staff = await _unitOfWork.StaffRepository.GetStaffByIdAsync(model.StaffId);
            if (staff == null)
            {
                _logger.LogWarning($"Staff member with ID {model.StaffId} not found.");
                throw new Exception("Staff member not found.");
            }

            staff.StaffNo = model.StaffNo;
            staff.FirstName = model.FirstName;
            staff.FamilyName = model.FamilyName;
            staff.PhoneHome = model.PhoneHome;
            staff.PhoneMobile = model.PhoneMobile;
            staff.Email = model.Email;
            staff.Address = new Address()
            {
                Line1 = model.Address.Line1,
                Line2 = model.Address.Line2,
                Suburb = model.Address.Suburb,
                TownOrCity = model.Address.TownOrCity,
                Postcode = model.Address.Postcode,
            };
            staff.IrdNumber = model.IrdNumber;
            staff.BankAccount = model.BankAccount;
            staff.PayrollId = model.PayrollId;
            staff.JobTitle = model.JobTitle;
            staff.WorkRole = model.WorkRole;
            staff.IsActive = model.IsActive;
            staff.ApplicationUserId = model.ApplicationUserId;
            staff.UpdatedAt = DateTime.UtcNow;
            staff.UpdatedById = updatedById;

            await _unitOfWork.StaffRepository.UpdateStaffAsync(staff);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> SoftDeleteStaffAsync(int staffId, string updatedById)
        {
            var staff = await _unitOfWork.StaffRepository.GetStaffByIdAsync(staffId);

            if (staff == null)
            {
                throw new KeyNotFoundException("Staff member not found.");
            }

            // Soft delete staff
            staff.IsDeleted = true;
            staff.IsActive = false;
            staff.UpdatedAt = DateTime.UtcNow;
            staff.UpdatedById = updatedById;

            await _unitOfWork.StaffRepository.UpdateStaffAsync(staff);

            return true;
        }

        // Assigns a login account to a staff member (via UserService)
        public async Task<bool> AssignApplicationUserAsync(int staffId, string email, string password)
        {
            var staff = await _unitOfWork.StaffRepository.GetStaffByIdAsync(staffId);
            if (staff == null)
            {
                _logger.LogWarning($"Staff with ID {staffId} not found for user assignment.");
                return false;
            }

            if (!string.IsNullOrEmpty(staff.ApplicationUserId))
            {
                _logger.LogWarning($"Staff ID {staffId} already has an associated ApplicationUser.");
                return false;
            }

            var (result, applicationUserId) = await _userService.CreateUserLoginAsync(email, password);
            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to create application user for staff ID {staffId}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return false;
            }

            staff.ApplicationUserId = applicationUserId;
            await _unitOfWork.StaffRepository.UpdateStaffAsync(staff);

            _logger.LogInformation($"Successfully assigned ApplicationUserId {applicationUserId} to Staff ID {staffId}.");
            return true;
        }

        public async Task UpdateStaffApplicationUserId(int staffId, string applicationUserId)
        {
            var staff = await _unitOfWork.StaffRepository.GetStaffByIdAsync(staffId);

            if (staff == null)
            {
                throw new KeyNotFoundException($"Staff member with ID {staffId} not found.");
            }

            staff.ApplicationUserId = applicationUserId;
            await _unitOfWork.StaffRepository.UpdateStaffAsync(staff);
        }

    }
}
