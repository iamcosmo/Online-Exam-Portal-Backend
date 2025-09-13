using Domain.Models;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionsController(IQuestionRepository repo)
        {
            _questionRepository = repo;
        }

        [HttpGet("questions-index")]
        public IActionResult Index()
        {
            return Ok("Index Page for Exam Controller");
        }

        [Authorize(Roles = "Student")]
        [HttpPost("add-question")]
        public async Task<IActionResult> AddQuestion([FromBody] Question question, [FromQuery] int examId)
        {
            // Fetch the Exam entity using examId (assuming you have a repository/service for this)
            Exam exam = new Exam { Eid = examId }; // Replace with actual fetch if available

            var result = await _questionRepository.AddQuestion(question, examId);
            return result > 0 ? Ok("Question added successfully") : BadRequest("Failed to add Question");
        }

        //[HttpPut("update-exam")]
        //public IActionResult UpdateExam([FromBody] Exam exam)
        //{
        //    var result = _examRepository.UpdateExam(exam);
        //    return result > 0 ? Ok("Exam updated successfully") : NotFound("Exam not found or no changes made");
        //}
    }
}
