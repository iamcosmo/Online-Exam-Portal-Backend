using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;


        public AuthController(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }


        // POST: api/auth/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var result = _userRepository.RegisterUser(user);
            return result > 0 ? Ok("User registered successfully") : BadRequest("Registration failed");
        }


        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _userRepository.Login(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            string token = _tokenService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        

    }
}
