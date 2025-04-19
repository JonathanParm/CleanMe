using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using CleanMe.Application.Interfaces;
using CleanMe.Domain.Entities;
using CleanMe.Shared.Models;

namespace CleanMe.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<(IdentityResult, string?)> CreateUserAsync(string email, string password)
        {
            var user = new ApplicationUser { Email = email, UserName = email };
            var result = await _userManager.CreateAsync(user, password);

            return (result, result.Succeeded ? user.Id : null);
        }

        public async Task<(IdentityResult, string)> CreateUserLoginAsync(string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to create user with email {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return (result, null); // Return null ApplicationUserId on failure
            }

            return (IdentityResult.Success, user.Id); // Return the new ApplicationUserId on success
        }

        public async Task<IdentityResult> UpdateUserLoginAsync(string userId, string email, string? newPassword)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
            {
                Console.WriteLine($"ERROR: User with ID {userId} not found.");
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Update Email & Username
            appUser.UserName = email;
            appUser.Email = email;
            appUser.NormalizedEmail = email.ToUpper();
            appUser.NormalizedUserName = email.ToUpper();

            var updateResult = await _userManager.UpdateAsync(appUser);
            if (!updateResult.Succeeded)
            {
                Console.WriteLine("ERROR: Failed to update user details.");
                foreach (var error in updateResult.Errors)
                {
                    Console.WriteLine($"ERROR DETAIL: {error.Description}");
                }
                return updateResult;
            }

            // Change Password if provided
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine($"DEBUG: Attempting to reset password for User ID: {userId}");
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                var resetResult = await _userManager.ResetPasswordAsync(appUser, passwordResetToken, newPassword);

                if (!resetResult.Succeeded)
                {
                    Console.WriteLine("ERROR: Password reset failed.");
                    foreach (var error in resetResult.Errors)
                    {
                        Console.WriteLine($"ERROR DETAIL: {error.Description}");
                    }
                    return resetResult;
                }

                Console.WriteLine("DEBUG: Password reset succeeded.");
            }

            return IdentityResult.Success;
        }

        public async Task<ApplicationUser?> GetApplicationUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task DisableUserLoginAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning($"Attempted to disable non-existent user: {userId}");
                return;
            }

            // Lock user account indefinitely (user cannot log in)
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to disable user login for UserId: {userId}");
            }
        }

    }
}