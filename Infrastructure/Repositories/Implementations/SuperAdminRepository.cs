using Domain.Data;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Services;
using Infrastructure.DTOs.adminDTOs;

namespace Infrastructure.Repositories.Implementations
{
    public class SuperAdminRepository : ISuperAdminRepository
    {
        private readonly AppDbContext _context;
        private readonly TokenService tokenService;

        public SuperAdminRepository(AppDbContext context, TokenService token)
        {
            _context = context;
            tokenService = token;
        }

        public async Task<string> GenerateTokenAsync(string role, string email)
        {
            var JwtToken = tokenService.GenerateJwtTokenForRegistration(role, email);
            var validation = new Validation
            {
                Token = JwtToken,
            };

            _context.Validations.Add(validation);
            _context.SaveChanges();
            return JwtToken;
        }

        public async Task<IEnumerable<AdminListDto>> GetAllAdminsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Admin") //&& !u.IsDeleted)
                .Select(u => new AdminListDto
                {
                    Id = u.UserId,
                    Name = u.FullName,
                    Email = u.Email,
                    //IsBlocked = !u.IsActive
                }).ToListAsync();
        }

        public async Task<bool> BlockAdminAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Role != "Admin") return false;

            user.IsBlocked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockAdminAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Role != "Admin") return false;

            user.IsBlocked = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Replace the static method with an instance method to access _context
        public bool ValidateToken(string token)
        {
            return _context.Validations.Any(v => v.Token == token);
        }

        // Replace the static RemoveToken method with an instance method and fix the argument type
        public void RemoveToken(string token)
        {
            var validation = _context.Validations.FirstOrDefault(v => v.Token == token);
            if (validation != null)
            {
                _context.Validations.Remove(validation);
                _context.SaveChanges();
            }
        }
    }

}
