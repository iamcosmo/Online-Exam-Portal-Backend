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
        public async Task<IActionResult> AddExamAction([FromBody] AddExamDTO dto)
        {
            Exam exam = new Exam
            {

                Name = dto.Name,
                Description = dto.Description,
                TotalQuestions = dto.TotalQuestions,
                TotalMarks = dto.TotalMarks,
                Duration = dto.Duration,
                Tids = dto?.Tids,
                DisplayedQuestions = dto.DisplayedQuestions

            };
            var result = await _examRepository.AddExam(exam);
            return result > 0 ? Ok("Exam added successfully") : BadRequest("Failed to add exam");
        }

        [Authorize(Roles = "Examiner")]
        [HttpPut("update-exam")]
        public IActionResult UpdateExamAction([FromBody] AddExamDTO dto)
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

        [Authorize(Roles = "Examiner")]
        [HttpDelete("delete-exam/{examid}")]
        public IActionResult DeleteExamAction(int examid)
        {
            int status = _examRepository.DeleteExam(examid);
            if (status > 1) return Ok("All recors related to the exams also deleted.");
            else if (status == -1) return NotFound("Exam not found.");
            else return StatusCode(500);
        }

        [Authorize(Roles = "Examiner")]
        [HttpGet("get-exams/e")]
        public IActionResult GetExamsForExaminerAction([FromQuery] int userid)
        {
            var exams = _examRepository.GetExamsForExaminer(userid);
            if (exams == null) return Ok("No exams available.");
            return Ok(exams);
        }

        [Authorize(Roles = "Examiner")]
        [HttpGet("get-exams/e/{id}")]
        public IActionResult GetExamByIdForExaminerAction(int id)
        {
            var exam = _examRepository.GetExams(id);
            return exam != null ? Ok(exam) : NotFound("Exam not found");
        }



        [Authorize(Roles = "Student")]
        [HttpGet("get-exams")]
        public IActionResult GetExamsAction()
        {
            var exams = _examRepository.GetExams();
            if (exams == null) return Ok("No exams available.");
            return Ok(exams);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("get-exams/{id}")]
        public IActionResult GetExamByIdAction(int id)
        {
            var exam = _examRepository.GetExams(id);
            return exam != null ? Ok(exam) : NotFound("Exam not found");
        }

        [Authorize(Roles = "Student")]
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

        [Authorize(Roles = "Student")]
        [HttpPost("submit-exam")]
        public IActionResult SubmitExamAction(SubmittedExamDTO examdto)
        {
            var status = _examRepository.SubmitExam(examdto);

            return Ok("Status Returned: " + status);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("view-exam-results/{examid}")]
        public IActionResult ViewExamResultsAction([FromRoute] int examid, [FromQuery] int userid)
        {
            var attemptedExams = _examRepository.ViewExamResults(examid, userid);
            return Ok(attemptedExams);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("create-results/{examid}")]
        public IActionResult CreateExamResultsAction([FromRoute] int examid, [FromQuery] int userid)
        {
            var status = _examRepository.CreateExamResults(examid, userid);
            return status > 0 ? Ok("Result created") : StatusCode(500, "Result Could not be created.");
        }
    }
}
