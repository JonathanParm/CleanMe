using System.Collections.Generic;
using System.Threading.Tasks;
using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IStaffService
    {
        // Retrieves a paginated & filtered staff list using Dapper
        Task<IEnumerable<StaffIndexViewModel>> GetStaffIndexAsync(
            string? staffNo, string? fullName, string? workRole, string? contactDetail, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);

        Task<IEnumerable<StaffViewModel>> FindDuplicateStaffAsync(string firstName, string familyName, int? staffNo, int? excludeStaffId);
        Task<bool> IsEmailAvailableAsync(string email, int? staffId);

        Task<StaffViewModel?> GetStaffViewModelByIdAsync(int staffId);
        Task<int> AddStaffAsync(StaffViewModel model, string addedById);
        Task UpdateStaffAsync(StaffViewModel model, string updatedById);
        Task<bool> SoftDeleteStaffAsync(int staffId, string updatedById);
        Task UpdateStaffApplicationUserId(int staffId, string applicationUserId);

    }
}