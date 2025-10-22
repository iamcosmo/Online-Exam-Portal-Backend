using Domain.Models;
using Infrastructure.DTOs.AuthDTOs;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OEP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly TokenService _tokenService;



        public AuthController(IAuthRepository authRepository, TokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }


        // POST: api/auth/register
        [HttpPost("student/register")]
        public IActionResult RegisterStudentAction([FromBody] RegisterUserDTO dto)
        {
            var result = _authRepository.RegisterStudent(dto);
            return result.status > 0 ? Ok(new { msg = "User Saved! Please verify Email via OTP-verification", result.UserId })
    : BadRequest(new { error = "Registration failed" });

            //? Ok("User Saved! Please verify Email via OTP-verification") : BadRequest("Registration failed");
        }

        [HttpPost("internal/register")]
        public IActionResult RegisterAction([FromBody] RegistrationInputDTO inputdto)
        {
            string tokenEncrypted = inputdto.Token;
            if (!_authRepository.ValidateToken(tokenEncrypted))
            {
                BadRequest("Input Details are incorrect.");
            }
            // Use the injected service to get the role
            string? userRole = _tokenService.GetRoleFromToken(tokenEncrypted);


            string role = !string.IsNullOrEmpty(userRole) ? userRole : "Examiner";

            var result = _authRepository.RegisterAdminOrExaminer(inputdto, role);

            //return result > 0 ? Ok("User registered successfully") : BadRequest("Registration failed");

            return result > 0
                ? Ok(new { message = "User registered successfully", role = role })
                : BadRequest(new { error = "Registration failed" });

        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var user = _authRepository.Login(dto.Email, dto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials Or You are Blocked Or Verify your Email.");
            }

            string token = _tokenService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("student/verifyotp/{userId}")]
        public IActionResult VerifyOTP([FromBody] int otp, int userId)
        {
            if (_authRepository.VerifyOTP(userId, otp))
                return Ok("OTP verified successfully");

            return Unauthorized("Incorrect OTP!!");
        }

        [HttpGet("student/resendotp/{userId}")]
        public IActionResult ResendOTP(int userId)
        {
            int result = _authRepository.ResendOTP(userId);
            return result > 0 ? Ok("OTP Resent Successfully") : BadRequest("Failed to resend OTP");
        }


    }
}
