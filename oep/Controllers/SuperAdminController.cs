using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;
using Infrastructure.DTOs.adminDTOs;

namespace OEP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuperAdminController : ControllerBase
    {
        private readonly ISuperAdminRepository _superAdminRepo;

        public SuperAdminController(ISuperAdminRepository superAdminRepo)
        {
            _superAdminRepo = superAdminRepo;
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateTokenDTO tokenDTO)
        {
            var tok = await _superAdminRepo.GenerateTokenAsync(tokenDTO.role, tokenDTO.email);
            return Ok(new { Token = tok });
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _superAdminRepo.GetAllAdminsAsync();
            return Ok(admins);
        }

        [HttpPut("block/{id}")]
        public async Task<IActionResult> BlockAdmin(int id)
        {
            var result = await _superAdminRepo.BlockAdminAsync(id);
            return result ? Ok("Admin blocked.") : NotFound("Admin not found.");
        }

        [HttpPut("unblock/{id}")]
        public async Task<IActionResult> UnblockAdmin(int id)
        {
            var result = await _superAdminRepo.UnblockAdminAsync(id);
            return result ? Ok("Admin unblocked.") : NotFound("Admin not found.");
        }
    }

}
