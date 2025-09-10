using Domain.Models;
using Infrastructure.Repositories.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ExamsController : Controller
    {
        private readonly ExamRepository _examRepository;

        public ExamsController(AppDbContext context)
        {
            _examRepository = new ExamRepository(context);
        }
        public IActionResult Index()
        {
            return Ok("Index na");
        }


        [HttpPost("add-exam")]
        public IActionResult AddExam([FromBody] Exam exam)
        {
            var result = _examRepository.AddExam(exam);
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

    }
}
