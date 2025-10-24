using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.adminDTOs;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.DTOs.QuestionFeedbackDTO;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OEP.Controllers
{

    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]

    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
      

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }


        [HttpGet("approve-exam-list")]
        public async Task<IActionResult> ToBeApprovedExamListAction([FromQuery] int userid)
        {
            var examList = await _adminRepository.ExamsToBeApprovedList(userid);
            if (examList == null || !examList.Any())
            {
                return Ok(new { message = "No Exams to be Approved.", ExamList = new List<Exam>() });
            }
            else
            {
                return Ok(new { ExamList = examList });
            }
        }


        // Replace the ApproveOrRejectExam method with the following:

        // POST /approve-exam/{eid}/{action}
        [HttpPost("approve-exam")]
        //[HttpPost("approve-exam/{eid}")]
        public async Task<IActionResult> ApproveOrRejectExam([FromBody] ExamApprovalStatusDTO dto)
        {

            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest("Invalid request data.");
            }
            

            int result = await _adminRepository.ApproveExamAsync(dto);

            if (result >= 1)
            {
                return Ok(new {message= $"Exam with ID {dto.ExamId} has been {(dto.Status.ToLower() == "approve" ? "approved" : "rejected")}." });
            }
            else
            {
                return StatusCode(500, "Failed to update exam status.");
            }

        }




        [HttpGet("reported-questions")]
        public async Task<IActionResult> GetAllReportedQuestions([FromQuery] int adminId)
        {
            var questions = await _adminRepository.GetAllReportedQuestionsAsync(adminId);

            if (questions == null || !questions.Any())
            {
                return Ok(new { msg = "No reported questions found." });
            }

            return Ok(questions);
        }

        [HttpGet("review-questions/{qid}")]
        public async Task<IActionResult> GetReportedQuestionById(int qid)
        {
            var question = await _adminRepository.GetReportedQuestionByIdAsync(qid);
            if (question == null)
                return Ok(new { msg = "No reported questions found." });
            return Ok(question);
        }

        // POST /admin/review-questions/{qid}/{status}
        [HttpPost("review-questions")]
        public async Task<IActionResult> ReviewReportedQuestion([FromBody] QuestionReviewDTO dto)
        {
            if (dto.status != 0 && dto.status != 1)
                return BadRequest("Invalid status. Use 0 or 1.");

            var result = await _adminRepository.UpdateReportedQuestionStatusAsync(dto);

            if (!result)
                return Ok("No reported questions found.");

            return Ok($"Question with ID {dto.qid} has been {(dto.status == 1 ? "approved" : "rejected")}.");
        }


        [HttpPut("blockuser/{id}")]

        public async Task<IActionResult> BlockUser(int id)

        {

            var result = await _adminRepository.BlockUserAsync(id);

            if (result.Contains("not found"))

                return NotFound(result);

            if (result.Contains("not allowed"))

                return Unauthorized(result);

            if (result.Contains("already blocked"))

                return BadRequest(result);

            return Ok(result);

        }


        // GET /exam-feedback-review/{eid}
        [HttpGet("exam-feedback-review")]
        public async Task<IActionResult> GetExamFeedbacks([FromQuery] int userId)
        {
            var feedbacks = await _adminRepository.GetExamFeedbacksAsync(userId);
            return feedbacks != null ? Ok(feedbacks) : NotFound("No feedbacks found.");
        }

        [HttpPost("toggle-user-status")]
        public async Task<IActionResult> ToggleUserStatus([FromBody] ToggleUserStatusDto dto)
        {
            var result = await _adminRepository.ToggleUserStatusAsync(dto.UserId, dto.IsActive);
            return result >= 1 ? Ok($"User {(dto.IsActive ? "activated" : "deactivated")} successfully.") : BadRequest("Operation failed.");
        }
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminRepository.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpPost("add-adminremarks/{examId}")]
        public async Task<IActionResult> AddAdminRemarksAction(int examId, [FromBody] RemarkDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Remarks)) ;
            var status = await _adminRepository.AddAdminRemarksAsync(examId, dto.Remarks);
            return status > 0 ? Ok(new { message = "Admin Remarks added" }) : BadRequest(new { message = "Some error occured while adding remarks" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("topic-list")]
        public async Task<IActionResult> TopicsToBeApprovedAction([FromQuery] int userId)
        {
            var topics = await _adminRepository.TopicsToBeApprovedAsync(userId);
            if (topics == null)
                return Ok(new { Message = "No topics Found For approval" });

            return Ok(new { Topics = topics });
        }

        // ✅ ADMIN — Get Exam With Questions For Review

        [Authorize(Roles = "Admin")]

        [HttpGet("exam/{examId}/review")]

        public async Task<IActionResult> GetExamWithQuestionsForReview(int examId)

        {

            try

            {

                var exam = await _adminRepository.GetExamWithQuestionsForAdminAsync(examId);

                if (exam == null)

                {

                    return NotFound(new { message = "Exam not found or not submitted for review." });

                }

                var response = new

                {

                    examId = exam.Eid,

                    name = exam.Name,

                    description = exam.Description,

                    totalMarks = exam.TotalMarks,

                    questions = exam.Questions.Select(q => new

                    {

                        q.Qid,

                        q.Question1,           // ✅ correct name from your Question model

                        q.Type,

                        q.Options,            // ✅ this is a JSON string

                        q.CorrectOptions,     // ✅ property name from your Question model

                        q.Marks

                    })

                };

                return Ok(response);

            }

            catch (Exception ex)

            {

                return StatusCode(500, new { message = "Error while fetching exam details.", error = ex.Message });

            }

        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("approve-topic")]
        public async Task<IActionResult> ApproveOrRejectTopicAction([FromBody] TopicActionDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Action))
            {
                return BadRequest(new { Message = "Invalid request data." });
            }
            var status = await _adminRepository.ApproveOrRejectTopic(dto.TopicId, dto.UserId, dto.Action);
            if (status == 1)
                return Ok(new { Message = $"Topic {dto.Action}d Successfully." });

            return NotFound(new { Message = "Topic not found or update failed." });
        }
    }





}
