using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using CleanMe.Domain.Entities;
using CleanMe.Shared.Models;

namespace CleanMe.Application.Interfaces
{
    public interface IUserService
    {
        Task<(IdentityResult, string)> CreateUserLoginAsync(string email, string password);
        Task<IdentityResult> UpdateUserLoginAsync(string userId, string email, string? newPassword);
        
        Task<ApplicationUser?> GetApplicationUserByIdAsync(string userId);
        Task DisableUserLoginAsync(string userId);
    }
}