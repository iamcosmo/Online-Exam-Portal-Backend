using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

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
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserDTO dto)
        {
            if (dto.Role == "Admin" || dto.Role == "Examiner")
            {
                return Unauthorized("Only student are allowed to register.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role,
                PhoneNo = dto.PhoneNo
            };

            var result = _authRepository.RegisterUser(user);
            return result > 0 ? Ok("User registered successfully") : BadRequest("Registration failed");
        }


        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var user = _authRepository.Login(dto.Email, dto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            string token = _tokenService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }


    }
}
