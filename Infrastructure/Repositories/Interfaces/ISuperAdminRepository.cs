using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DTOs.adminDTOs;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ISuperAdminRepository
    {
        /// <summary>
        /// Generates a one-time token for admin registration and stores it in memory.
        /// If the user does not exist, creates a new user with Role = "Admin" and IsActive = false.
        /// </summary>
        /// <param// n//ame="email">Email of the admin to invite</param>
        /// <returns>Generated token string</returns>
        Task<string> GenerateTokenAsync(string role, string email);

        /// <summary>
        /// Returns a list of all users with Role = "Admin"
        /// </summary>
        /// <returns>List of AdminListDto</returns>
        Task<IEnumerable<AdminListDto>> GetAllAdminsAsync();

        /// <summary>
        /// Blocks an admin by setting IsActive = false
        /// </summary>

        /// <returns>True if successful, false if not found or not an admin</returns>
        Task<bool> BlockAdminAsync(int userId);

        /// <summary>
        /// Unblocks an admin by setting IsActive = true
        /// </summary>
        /// <param //name="userId">User ID of the admin</param>
        /// <returns>True if successful, false if not found or not an admin</returns>
        Task<bool> UnblockAdminAsync(int userId);

        bool ValidateToken(string token);

        void RemoveToken(string token);

    }
    //public interface ISuperAdminRepository
    //{
    //   // Task AddAdminAsync(AdminCreateDto dto);
    //   // Task<bool> RemoveAdminAsync(int id);
    //    Task<bool> BlockAdminAsync(int id);
    //    Task<bool> UnblockAdminAsync(int id);
    //    Task<IEnumerable<AdminListDto>> GetAllAdminsAsync();
    //}

}
