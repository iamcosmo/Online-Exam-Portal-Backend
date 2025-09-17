using Domain.Models;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize(Roles = "Examiner")]
        [HttpPost("add-exam")]
        public async Task<IActionResult> AddExamAction([FromBody] AddExamDTO dto)
        {
            var result = await _examRepository.AddExam(dto);
            return result > 0 ? Ok("Exam added successfully") : BadRequest("Failed to add exam");
        }

        [Authorize(Roles = "Examiner")]
        [HttpPut("update-exam/{examId}")]
        public IActionResult UpdateExamAction([FromRoute] int examId, [FromBody] UpdateExamDTO dto)
        {

            var result = _examRepository.UpdateExam(examId, dto);
            return result > 0 ? Ok("Exam updated successfully") : StatusCode(500, "Exam was not updated due to Internal Errors.");
        }

        [Authorize(Roles = "Examiner")]
        [HttpDelete("delete-exam/{examid}")]
        public IActionResult DeleteExamAction(int examid)
        {
            int status = _examRepository.DeleteExam(examid);
            if (status >= 1) return Ok("Exam Deleted Successfully.");
            else if (status == -1) return NotFound("Exam not found.");
            else return StatusCode(500, "Some error Occured,status value =" + status);
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


    }
}
