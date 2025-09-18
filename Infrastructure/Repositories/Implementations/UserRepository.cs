using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.UserDTOs;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }


        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }

        public List<User> GetUsersByRole(string role)
        {
            return _context.Users.Where(u => u.Role == role).ToList();
        }

        public int UpdateUser(int id, UpdateUserDTO dto)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == dto.Id);
            if (existingUser == null)
                return 0;

            if (!string.IsNullOrEmpty(dto.FullName))
                existingUser.FullName = dto.FullName;

            if (dto.Dob != null)
                existingUser.Dob = dto.Dob;

            if (!string.IsNullOrEmpty(dto.PhoneNo))
                existingUser.PhoneNo = dto.PhoneNo;

            return _context.SaveChanges();
        }


        public int DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
                return 0;

            _context.Users.Remove(user);
            return _context.SaveChanges();
        }


    }
}



