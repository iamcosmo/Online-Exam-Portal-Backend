using Domain.Data;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;


namespace Infrastructure.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }
        public int RegisterUser(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges();
        }
        public User? Login(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
    }
}
