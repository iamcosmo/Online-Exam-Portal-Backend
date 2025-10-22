using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.AuthDTOs;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
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

        private readonly EmailService _emailService;

        private readonly ILogger<AuthRepository> _logger;
        public AuthRepository(AppDbContext context, ILogger<AuthRepository> logger, EmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        public int RegisterAdminOrExaminer(RegistrationInputDTO examinerDTO, string role)
        {

            var user = new User
            {
                FullName = examinerDTO.FullName,
                Email = examinerDTO.Email,
                Password = examinerDTO.Password,
                Role = role,
                PhoneNo = examinerDTO.PhoneNo,
                Dob = examinerDTO.Dob,
            };

            _context.Users.Add(user);
            _logger.LogInformation("New Admin/Examiner is registered with UserId : {@userid} at {@time}", user.UserId, user.CreatedAt);
            return _context.SaveChanges();
        }
        public StudentRegisterResponseDTO RegisterStudent(RegisterUserDTO dto)
        {

            Random random = new Random();
            int newOtp = random.Next(100000, 999999);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password,
                Role = "Student",
                PhoneNo = dto.PhoneNo,
                Dob = dto.Dob,
                Otp = newOtp
            };
            _context.Users.Add(user);


            string emailSubject = "Verify Your Email";
            string emailBody = $"Dear {dto.FullName},\n\nYour OTP for email verification is: {newOtp}\n\nThank you!";

            try
            {
                _emailService.SendSimpleEmail(dto.Email, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send OTP email to {Email}. Error: {ErrorMessage}", dto.Email, ex.Message);
                return new StudentRegisterResponseDTO { status = 0, UserId = -1 };
            }
            _logger.LogInformation("New Student registeration Started with UserId : {@userid} at {@time}. Verify Email!", user.UserId, user.CreatedAt);
            int status = _context.SaveChanges();
            return new StudentRegisterResponseDTO { status = status, UserId = user.UserId };
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
                string emailSubject = "Email Verified Successfully";
                string emailBody = $"Dear {user.FullName},\n\nYour Email OTP verification was successful.\nYou are officially Verified Now!!\n\nThank you!";

                try
                {
                    _emailService.SendSimpleEmail(user.Email, emailSubject, emailBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to send confirmation email to {Email}. Error: {ErrorMessage}", user.Email, ex.Message);
                    return false;
                }
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
