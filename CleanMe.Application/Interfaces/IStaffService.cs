using CleanMe.Application.Helpers.Paging;
using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Interfaces
{
    public interface IStaffService
    {
        // Retrieves a paginated & filtered staff list using Dapper
        Task<IEnumerable<StaffIndexViewModel>> GetStaffIndexAsync(
            string? staffId, string? fullName, string? workRole, string? contactDetail, string? isActive,
            string sortColumn, string sortOrder, int pageNumber, int pageSize);

        Task<IEnumerable<StaffViewModel>> FindDuplicateStaffAsync(string firstName, string familyName, int? staffId);
        //Task<IEnumerable<StaffViewModel>> FindDuplicateStaffAsync(string firstName, string familyName, int? staffId, int? excludeStaffId);
        Task<bool> IsEmailAvailableAsync(string email, int? staffId);

        Task<StaffViewModel?> GetStaffViewModelByIdAsync(int staffId);
        Task<int> AddStaffAsync(StaffViewModel model, string addedById);
        Task UpdateStaffAsync(StaffViewModel model, string updatedById);
        Task<bool> SoftDeleteStaffAsync(int staffId, string updatedById);
        Task UpdateStaffApplicationUserId(int staffId, string applicationUserId);
        Task<PagedResult<StaffIndexViewModel>> GetPagedIndexAsync(
            int pageNumber,
            int pageSize,
            string sortColumn,
            string sortOrder,
            string? staffId,
            string? FullName,
            string? WorkRole,
            string? ContactDetail,
            string? isActive
        );
    }
}