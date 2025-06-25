using Pawfectmatch_V._1.Models;

namespace Pawfectmatch_V._1.Services
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(ApplicationUser user, string password, string role);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<List<string>> GetUserRolesAsync(ApplicationUser user);
        Task<bool> AddUserToRoleAsync(ApplicationUser user, string role);
        Task<bool> RemoveUserFromRoleAsync(ApplicationUser user, string role);
        Task<bool> IsUserInRoleAsync(ApplicationUser user, string role);
    }
} 