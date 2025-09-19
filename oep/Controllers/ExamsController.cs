using Domain.Models;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public async Task<IActionResult> UpdateExamAction([FromRoute] int examId, [FromBody] UpdateExamDTO dto)
        {

            var result = await _examRepository.UpdateExam(examId, dto);
            return result > 0 ? Ok("Exam updated successfully") : StatusCode(500, "Exam was not updated due to Internal Errors.");
        }

        [Authorize(Roles = "Examiner")]
        [HttpDelete("delete-exam/{examid}")]
        public async Task<IActionResult> DeleteExamAction(int examid)
        {
            int status = await _examRepository.DeleteExam(examid);
            if (status >= 1) return Ok("Exam Deleted Successfully.");
            else if (status == -1) return NotFound("Exam not found.");
            else return StatusCode(500, "Some error Occured,status value =" + status);
        }

        [Authorize(Roles = "Examiner")]
        [HttpGet("get-exams/e")]
        public async Task<IActionResult> GetExamsForExaminerAction([FromQuery] int userid)
        {
            var exams = await _examRepository.GetExamsForExaminer(userid);
            if (exams == null) return Ok("No exams available.");
            return Ok(exams);
        }

        [Authorize(Roles = "Examiner")]
        [HttpGet("get-exams/e/{id}")]
        public async Task<IActionResult> GetExamByIdForExaminerAction(int id)
        {
            var exam = await _examRepository.GetExams(id);
            return exam != null ? Ok(exam) : NotFound("Exam not found");
        }

        [Authorize(Roles = "Examiner")]
        [HttpPatch("/approval-exam/{examId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubmitExamForApprovalAction(int examId)
        {
            var result = await _examRepository.SubmitExamForApproval(examId);
            return result > 0 ? Ok("Exam submitted for approval") : StatusCode(500, "Internal Server Error while submitting for approval.");
        }



        [Authorize(Roles = "Student")]
        [HttpGet("get-exams")]
        public async Task<IActionResult> GetExamsAction()
        {
            var exams = await _examRepository.GetExams();
            if (exams == null) return Ok("No exams available.");
            return Ok(exams);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("get-exams/{id}")]
        public async Task<IActionResult> GetExamByIdAction(int id)
        {
            var exam = await _examRepository.GetExams(id);
            return exam != null ? Ok(exam) : NotFound("Exam not found");
        }

        [Authorize(Roles = "Student")]
        [HttpPost("start-exam/{examId}")]
        public async Task<IActionResult> StartExamAction(int examId)
        {
            try
            {
                var StartExamData = await _examRepository.StartExam(examId);
                return Ok(new { ExamData = StartExamData, Success = true });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPost("submit-exam")]
        public async Task<IActionResult> SubmitExamAction(SubmittedExamDTO examdto)
        {
            var status = await _examRepository.SubmitExam(examdto);

            return Ok("Status Returned: " + status);
        }


    }
}
