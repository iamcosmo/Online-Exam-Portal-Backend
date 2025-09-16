using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class SuperAdminRepository : ISuperAdminRepository
    {
        private readonly AppDbContext _context;
        private static readonly Dictionary<string, string> _tokenStore = new(); // email -> token

        public SuperAdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateTokenAsync()
        {
            //var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            //if (existingUser == null)
            //{
            //    var user = new User
            //    {
            //        Email = email,
            //        Role = "Admin",
            //        //IsBlocked = false,
            //        //IsDeleted = false
            //    };
            //    _context.Users.Add(user);
            //    await _context.SaveChangesAsync();
            //}
            //var validation=new Validation
            var token = Guid.NewGuid().ToString();
            var validation = new Validation
            { 
                Token = token,
               
            };
            _context.Validations.Add(validation);
            _context.SaveChanges();
            return token;
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
