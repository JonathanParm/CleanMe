using CleanMe.Shared.Models;

namespace CleanMe.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser> GetByUserIdAsync(string id);
        Task<ApplicationUser> GetByUserEmailAsync(string email);
        Task<bool> AddUserAsync(ApplicationUser user);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(string id);
    }
}