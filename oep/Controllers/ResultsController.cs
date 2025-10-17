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
            var status = await _resultRepo.CreateExamResults(examid, userid);
            return status > 0 ? Ok("Result created") : StatusCode(500, "Result Could not be created.");
        }

        [HttpGet("all-results/{userid}")]
        public async Task<IActionResult> GetAllResultsForUserAction([FromRoute] int userid)
        {
            var results = await _resultRepo.GetAllResultsForUser(userid);
            return Ok(results);
        }
    }
}
