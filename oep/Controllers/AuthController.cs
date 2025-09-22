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


        public AuthController(IAuthRepository userRepository, TokenService tokenService)
        {
            _authRepository = userRepository;
            _tokenService = tokenService;
        }


        // POST: api/auth/register
        [HttpPost("student/register")]
        public IActionResult RegisterStudentAction([FromBody] RegisterUserDTO dto)
        {
            var result = _authRepository.RegisterStudent(dto);
            return result > 0 ? Ok("User registered successfully") : BadRequest("Registration failed");
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

            // Your existing logic
            string role = !string.IsNullOrEmpty(userRole) ? userRole : "Examiner";

            var result = _authRepository.RegisterAdminOrExaminer(inputdto, role);

            return result > 0 ? Ok("User registered successfully") : BadRequest("Registration failed");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var user = _authRepository.Login(dto.Email, dto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials Or You are Blocked.");
            }

            string token = _tokenService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }


    }
}
