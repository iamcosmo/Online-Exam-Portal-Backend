using Domain.Data;
using Domain.Models;
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

        public int RegisterUser(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges();
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

        public int UpdateUser(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser == null)
                return 0;

            _context.Entry(existingUser).CurrentValues.SetValues(user);
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

        public User? Login(int userId, string password)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId && u.Password == password);
        }

    }
}



