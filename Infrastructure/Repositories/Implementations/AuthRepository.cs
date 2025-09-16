using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Infrastructure.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public int RegisterAdminOrExaminer(RegistrationInputDTO examinerDTO, string role)
        {
            var user = new User
            {
                FullName = examinerDTO.FullName,
                Email = examinerDTO.Email,
                Password = examinerDTO.Password,
                Role = role,
                PhoneNo = examinerDTO.PhoneNo
            };

            _context.Users.Add(user);
            return _context.SaveChanges();
        }
        public int RegisterStudent(RegisterUserDTO dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password,
                Role = "Student",
                PhoneNo = dto.PhoneNo
            };
            _context.Users.Add(user);
            return _context.SaveChanges();
        }
        public User? Login(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public bool ValidateToken(string token)
        {
            return _context.Validations.Any(v => v.Token == token);
        }
    }
}
