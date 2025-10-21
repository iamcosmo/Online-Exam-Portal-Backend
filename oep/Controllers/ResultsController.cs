using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{
    [Authorize(Roles = "Student")]
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : Controller
    {
        private readonly IResultRespository _resultRepo;
        public ResultsController(IResultRespository repo)
        {
            _resultRepo = repo;
        }

        [HttpPost("view-exam-result/{examid}")]
        public async Task<IActionResult> ViewExamResultsAction([FromRoute] int examid, [FromQuery] int userid)
        {
            var attemptedExams = await _resultRepo.ViewExamResults(examid, userid);
            return Ok(attemptedExams);
        }

        [HttpPost("create-results/{examid}")]
        public async Task<IActionResult> CreateExamResultsAction([FromRoute] int examid, [FromQuery] int userid)
        {
            var response = await _resultRepo.CreateExamResults(examid, userid);
            return response.status > 0 ? Ok(response) : StatusCode(500, response);
        }

        [HttpGet("all-results/{userid}")]
        public async Task<IActionResult> GetAllResultsForUserAction([FromRoute] int userid)
        {
            var results = await _resultRepo.GetAllResultsForUser(userid);
            return Ok(results);
        }

        [HttpGet("calculate/{examId}/{userId}")]
        public async Task<IActionResult> CalculateAndGetResult(int examId, int userId)
        {
            var response = await _resultRepo.ExecuteAndGetAllResultsAsync(examId, userId);

            if (!response.Success)
            {
                return Ok(response);
            }

            return Ok(response);
        }
    }
}
