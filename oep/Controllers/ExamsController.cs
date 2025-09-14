using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
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
        public async Task<IActionResult> AddExam([FromBody] AddExamDTO dto)
        {
            Exam exam = new Exam
            {

                Name = dto.Name,
                Description = dto.Description,
                TotalQuestions = dto.TotalQuestions,
                TotalMarks = dto.TotalMarks,
                Duration = dto.Duration,
                Tids = dto.Tids,
                DisplayedQuestions = dto.DisplayedQuestions

            };
            var result = await _examRepository.AddExam(exam);
            return result > 0 ? Ok("Exam added successfully") : BadRequest("Failed to add exam");
        }

        [HttpPut("update-exam")]
        public IActionResult UpdateExam([FromBody] AddExamDTO dto)
        {
            Exam exam = new Exam
            {

                Name = dto.Name,
                Description = dto.Description,
                TotalQuestions = dto.TotalQuestions,
                TotalMarks = dto.TotalMarks,
                Duration = dto.Duration,
                Tids = dto.Tids,
                DisplayedQuestions = dto.DisplayedQuestions

            };
            var result = _examRepository.UpdateExam(exam);
            return result > 0 ? Ok("Exam updated successfully") : StatusCode(500, "Exam was not updated due to Internal Errors.");
        }

        [HttpGet("get-exams")]
        public IActionResult GetExamsAction()
        {
            var exams = _examRepository.GetExams();
            if (exams == null) return Ok("No exams available.");
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
                var StartExamData = _examRepository.StartExam(examId);
                return Ok(new { ExamData = StartExamData, Success = true });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("submit-exam/{examId}")]
        public IActionResult SubmitExamAction(SubmittedExamDTO examdto)
        {
            var status = _examRepository.SubmitExam(examdto);

            return Ok("Status Returned: " + status);
        }
    }
}
