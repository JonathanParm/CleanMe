// --- Namespace: CleanMe.Infrastructure.Repositories ---
using CleanMe.Application.DTOs;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Common;
using CleanMe.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CleanMe.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly string _connectionString;

        public StaffRepository(IDapperRepository dapperRepository, IConfiguration configuration)
        {
            _dapperRepository = dapperRepository;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<StaffIndexViewModel>> GetStaffIndexAsync(
            string? staffNo, string? fullName, string? workRole,
            string? contactDetail, string? isActive,
            string sortColumn, string sortOrder,
            int pageNumber, int pageSize)
        {
            var sql = "EXEC dbo.StaffGetIndexView @StaffNo, @FullName, @WorkRole, @ContactDetail, @IsActive, @SortColumn, @SortOrder, @PageNumber, @PageSize";

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

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<StaffIndexViewModel>(sql, parameters);
        }

        public async Task<Staff?> GetStaffByIdAsync(int staffId)
        {
            var sql = "dbo.StaffGetById";
            var parameters = new { staffId = staffId };

            var results = await _dapperRepository.QueryAsync<StaffWithAddressDto>(sql, parameters, CommandType.StoredProcedure);
            var row = results.FirstOrDefault();
            if (row == null) return null;

            return new Staff
            {
                staffId = row.staffId,
                StaffNo = row.StaffNo,
                FirstName = row.FirstName,
                FamilyName = row.FamilyName,
                PhoneHome = row.PhoneHome,
                PhoneMobile = row.PhoneMobile,
                Email = row.Email,
                Address = new Address
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
                ApplicationUserId = row.ApplicationUserId,
                IsDeleted = row.IsDeleted,
                AddedAt = row.AddedAt,
                AddedById = row.AddedById,
                UpdatedAt = row.UpdatedAt,
                UpdatedById = row.UpdatedById
            };
        }
    }
}
