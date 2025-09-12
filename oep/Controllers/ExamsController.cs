using Domain.Data;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ExamsController : Controller
    {
        private readonly IExamRepository _examRepository;

        public ExamsController(IExamRepository repo)
        {
            _examRepository = repo;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return Ok("Index Page for Exam Controller");
        }

        [Authorize(Roles = "Examiner")]
        [HttpPost("add-exam")]
        public async Task<IActionResult> AddExam([FromBody] Exam exam)
        {
            var result = await _examRepository.AddExam(exam);
            return result > 0 ? Ok("Exam added successfully") : BadRequest("Failed to add exam");
        }

        [HttpPut("update-exam")]
        public IActionResult UpdateExam([FromBody] Exam exam)
        {
            var result = _examRepository.UpdateExam(exam);
            return result > 0 ? Ok("Exam updated successfully") : NotFound("Exam not found or no changes made");
        }

        [HttpGet("get-exams")]
        public IActionResult GetExams()
        {
            var exams = _examRepository.GetExams();
            return Ok(exams);
        }

        [HttpGet("get-exams/{id}")]
        public IActionResult GetExamById(int id)
        {
            var exam = _examRepository.GetExamById(id);
            return exam != null ? Ok(exam) : NotFound("Exam not found");
        }

        [HttpPost("start-exam/{examId}")]
        public IActionResult StartExamAction(int examId)
        {
            try
            {
                return Ok("Working");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
