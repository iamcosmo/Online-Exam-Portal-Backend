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
    public class UsersController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;


        public UsersController(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpGet("adminindex")]
        public IActionResult Index()
        {
            return Ok("Admin Controller is Working");
        }

        // GET /users?role=admin
        [HttpGet]
        public IActionResult GetUsers([FromQuery] string? role = null)
        {
            var users = string.IsNullOrEmpty(role)
                ? _userRepository.GetAllUsers()
                : _userRepository.GetUsersByRole(role);

            return Ok(users);
        }

        // GET /users/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        // PATCH /users/{id}
        [HttpPatch("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
                return BadRequest("User ID mismatch");

            var result = _userRepository.UpdateUser(user);
            return result > 0 ? Ok("User updated successfully") : NotFound("User not found");
        }

        // DELETE /users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var result = _userRepository.DeleteUser(id);
            return result > 0 ? Ok("User deleted successfully") : NotFound("User not found");
        }

        //// GET /users/{id}/exams-attempted
        //[HttpGet("{id}/exams-attempted")]
        //public IActionResult GetExamsAttempted(string id)
        //{
        //    var exams = _userRepository.GetExamsAttemptedByUser(id);
        //    return Ok(exams);
        //}

        //// GET /users/{userId}/{examId}/attempts
        //[HttpGet("{userId}/{examId}/attempts")]
        //public IActionResult GetExamAttempts(string userId, int examId)
        //{
        //    var attempts = _userRepository.GetExamAttempts(userId, examId);
        //    return Ok(attempts);
        //}



        // POST: api/users/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var result = _userRepository.RegisterUser(user);
            return result > 0 ? Ok("User registered successfully") : BadRequest("Registration failed");
        }


        // POST: api/users/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _userRepository.Login(2, request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            //jwt token creation
            string token = _tokenService.GenerateJwtToken(user);

            return user != null ? Ok(new { Token = token }) : Unauthorized("Invalid credentials");
        }


    }
}



