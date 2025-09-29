using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.AuthDTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Security.Claims;


namespace Infrastructure.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        private readonly ILogger<AuthRepository> _logger;
        public AuthRepository(AppDbContext context, ILogger<AuthRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int RegisterAdminOrExaminer(RegistrationInputDTO examinerDTO, string role)
        {

            Random random = new Random();
            int newOtp = random.Next(100000, 999999);
           
            var user = new User
            {
                FullName = examinerDTO.FullName,
                Email = examinerDTO.Email,
                Password = examinerDTO.Password,
                Role = role,
                PhoneNo = examinerDTO.PhoneNo,
                Dob = examinerDTO.Dob,
                Otp = newOtp
            };

            _context.Users.Add(user);
            _logger.LogInformation("New Admin/Examiner is registered with UserId : {@userid} at {@time}", user.UserId, user.CreatedAt);
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
            _logger.LogInformation("New Student is registered with UserId : {@userid} at {@time}", user.UserId, user.CreatedAt);
            return _context.SaveChanges();
        }
        public User? Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user?.Otp != null)
            {
                _logger.LogWarning("User with userId {@userid} attempted to login without verifying OTP at {@time}", user.UserId, DateTime.Now);
                return null;
            }

            if (user.IsBlocked == true) return null;
            else
            {
                _logger.LogInformation("{@Role} with userId {@userid} logged in at {@time}", user.Role, user.UserId, user.CreatedAt);
                return user;
            }         

        }

        public bool ValidateToken(string token)
        {
            return _context.Validations.Any(v => v.Token == token);
        }

        public bool VerifyOTP(int userId, int otp)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId && u.Otp == otp);
            if (user != null)
            {
                user.Otp = null;
                user.RegistrationDate = DateOnly.FromDateTime(DateTime.Now);
                _context.SaveChanges();
                _logger.LogInformation("OTP verified for UserId : {@userid} at {@time}", user.UserId, DateTime.Now);
                return true;
            }
            return false;
        }

        public int ResendOTP(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                Random random = new Random();
                int newOtp = random.Next(100000, 999999);
                user.Otp = newOtp;
                _context.SaveChanges();
                _logger.LogInformation("OTP resent for UserId : {@userid} at {@time}", user.UserId, DateTime.Now);
                return newOtp;
            }
            return 0;
        }
    }
}
