using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.DTOs.UserDTOs;

namespace OEP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IExamRepository _examRepository;

        public UsersController(IUserRepository userRepository, IExamRepository examRepository)
        {
            _userRepository = userRepository;
            _examRepository = examRepository;
        }


        // GET /users?role=admin
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUsers([FromQuery] string role)
        {
            var users = _userRepository.GetUsersByRole(role);
            return Ok(users);
        }

        // GET /users/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        // PATCH /users/{id}
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public IActionResult UpdateUserAction(int id, [FromBody] UpdateUserDTO dto)
        {
            var result = _userRepository.UpdateUser(id, dto);
            return result > 0 ? Ok("User updated successfully") : NotFound("User not found");
        }

        // DELETE /users/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var result = _userRepository.DeleteUser(id);
            return result > 0 ? Ok("User deleted successfully") : NotFound("User not found");
        }

        // GET /users/{id}/exams-attempted
        [Authorize(Roles = "Student")]
        [HttpGet("{id}/exams-attempted")]
        public IActionResult GetExamsAttempted(int id)
        {
            var exams = _examRepository.GetExamsAttemptedByUser(id);
            return Ok(exams);
        }

        // GET /users/{userId}/{examId}/attempts
        [Authorize(Roles = "Student")]
        [HttpGet("{userId}/{examId}/attempts")]
        public IActionResult GetExamAttempts(int userId, int examId)
        {
            var attempts = _examRepository.GetExamAttempts(userId, examId);
            return Ok(attempts);
        }

    }
}



