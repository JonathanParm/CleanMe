using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IStaffRepository
    {
        Task<Staff?> GetStaffByIdAsync(int staffId);
        Task AddStaffAsync(Staff Staff);
        Task UpdateStaffAsync(Staff Staff);
    }
}
