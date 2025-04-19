using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;

namespace CleanMe.Application.Interfaces
{
    public interface IStaffRepository
    {
        // Executes stored procedure via Dapper
        Task<IEnumerable<StaffIndexViewModel>> GetStaffIndexAsync(
            string? staffNo, string? fullName, string? workRole, string? contactDetail, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);


        // Fetches a single staff record using EF Core
        Task<Staff?> GetStaffByIdAsync(int staffId);

        //Task AddStaffAsync(Staff staff, string addedById);
        //void Update(Staff staff, string updatedById);
    }
}
