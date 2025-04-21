using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface IStaffRepository
    {
        Task<IEnumerable<Staff>> GetAllStaffAsync(); 
        Task<Staff?> GetStaffByIdAsync(int staffId);
        Task AddStaffAsync(Staff Staff);
        Task UpdateStaffAsync(Staff Staff);
    }
}
